using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using StackExchange.Redis;
using Tgs.CashServiceWorker;
using Tgs.SerialNumberGenerator;
using TGS.GameProviders.Repositories.Entities;
using Tgs.CashService;

namespace Tgs.CashService.Repositories;

internal class WalletRepository(
    TgsContext dbContext,
    IDatabase redisDatabase,
    IDbScriptExecutor dbScriptExecutor,
    ISerialNumberGenerator serialNumberGenerator,
    ILogger<WalletRepository> logger)
    : IWalletRepository
{
    private static readonly JsonSerializerOptions _SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = false,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public async ValueTask<ExchangeResult> ExchangeAsync(
        Guid brandId,
        Guid walletId,
        string playerCode,
        TransactionType transactionType,
        decimal amount,
        decimal lockAmount,
        string? description,
        string? externalId,
        Guid? createdByUserId,
        CancellationToken cancellationToken = default)
    {
        var updatedOn = DateTime.UtcNow;
        var transactionId = await serialNumberGenerator
            .GenerateAsync(cancellationToken)
            .ConfigureAwait(false);

        var updateResult = await Policy
            .Handle<PostgresException>(ex => ex.ErrorCode == 40001)
            .WaitAndRetryAsync(3, count =>
            {
                var wait = Random.Shared.Next(100, 1000);

                logger.LogWarning("Exchange retry occur: {count}", count);
                return TimeSpan.FromMilliseconds(wait);
            })
            .ExecuteAsync(() => dbScriptExecutor.QueryFirstOrDefaultAsync<WalletBalanceReturn>(
                """
                    UPDATE dbo.wallets AS C
                    SET
                        balance = C.balance + @amount,
                        locked_balance = C.locked_balance + @lockAmount,
                        updated_on = @updatedOn
                    FROM
                        dbo.wallets AS O
                    WHERE
                        C.id = O.id
                        AND
                        C.id = @walletId
                    RETURNING
                        O.balance AS OriginBalance,
                        O.locked_balance AS OriginLockedBalance,
                        C.balance AS PostBalance,
                        C.locked_balance AS PostLockedBalance
                    """,
                new
                {
                    walletId,
                    amount,
                    lockAmount,
                    updatedOn
                }))
            .ConfigureAwait(false)
            ?? throw new WalletExchangeFailException();

        if (updateResult.PostLockedBalance < 0M)
        {
            updateResult = await Policy
                .Handle<PostgresException>(ex => ex.ErrorCode == 40001)
                .WaitAndRetryAsync(3, count =>
                {
                    var wait = Random.Shared.Next(100, 1000);

                    logger.LogWarning("Exchange retry occur: {count}", count);
                    return TimeSpan.FromMilliseconds(wait);
                })
                .ExecuteAsync(() => dbScriptExecutor.QueryFirstOrDefaultAsync<WalletBalanceReturn>(
                    """
                        UPDATE dbo.wallets AS C
                        SET
                            balance = C.balance + @amount,
                            locked_balance = C.locked_balance + @lockAmount,
                            updated_on = @updatedOn
                        FROM
                            dbo.wallets AS O
                        WHERE
                            C.id = O.id
                            AND
                            C.id = @walletId
                        RETURNING
                            O.balance AS OriginBalance,
                            O.locked_balance AS OriginLockedBalance,
                            C.balance AS PostBalance,
                            C.locked_balance AS PostLockedBalance
                        """,
                    new
                    {
                        walletId,
                        amount = -amount,
                        lockAmount = -lockAmount,
                        updatedOn
                    }))
                .ConfigureAwait(false);

            throw new WalletExchangeFailException();
        }

        _ = await dbContext.Transactions
            .AddAsync(new()
            {
                Id = transactionId,
                PlayerCode = playerCode,
                WalletId = walletId,
                BrandId = brandId,
                Amount = amount,
                LockAmount = lockAmount,
                Balance = updateResult.PostBalance,
                LockedBalance = updateResult.PostLockedBalance,
                Type = (short)transactionType,
                Description = description,
                RecordedOn = updatedOn,
                ExternalTransactionId = externalId,
                CreatedByUserId = createdByUserId
            }, cancellationToken)
            .ConfigureAwait(false);

        _ = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new ExchangeResult(
            transactionId,
            updateResult.PostBalance,
            updateResult.PostLockedBalance);
    }

    public async ValueTask<decimal?> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var playerWallet = await dbContext.WalletItems
            .Where(x => x.Id == walletId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return playerWallet?.Balance;
    }

    public async ValueTask<Wallet?> GetWalletAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var wallet = await dbContext.WalletItems.FindAsync(
            [walletId],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (wallet is null)
            return null;

        if (wallet.BrandId is null)
        {
            var player = await dbContext.Players.FindAsync(
                [wallet.PlayerCode],
                cancellationToken: cancellationToken).ConfigureAwait(false);

            return player is null
                ? null
                : new Wallet(
                    wallet.Id,
                    player.BrandId,
                    player.PlayerCode,
                    wallet.Balance,
                    wallet.LockedBalance,
                    wallet.SnapshotId);
        }

        return new Wallet(
            wallet.Id,
            wallet.BrandId!.Value,
            wallet.PlayerCode,
            wallet.Balance,
            wallet.LockedBalance,
            wallet.SnapshotId);
    }

    public async ValueTask WriteTransactionRecordAsync(TransactionRecord record, CancellationToken cancellationToken = default)
    {
        var key = new RedisKey($"Tgs:Transactions:{record.WalletId}");

        _ = await redisDatabase.ListRightPushAsync(
            key,
            JsonSerializer.Serialize(record, _SerializerOptions)).ConfigureAwait(false);
    }
}
