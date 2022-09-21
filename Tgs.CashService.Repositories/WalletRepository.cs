using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Tgs.CashServiceWorker;
using Tgs.SerialNumberGenerator;
using TGS.GameProviders.Repositories.Entities;

namespace Tgs.CashService.Repositories;

internal class WalletRepository : IWalletRepository
{
    private readonly TgsContext _context;
    private readonly IDbScriptExecutor _dbScriptExecutor;
    private readonly ISerialNumberGenerator _serialNumberGenerator;
    private readonly ILogger<WalletRepository> _logger;

    public WalletRepository(
        TgsContext context,
        IDbScriptExecutor dbScriptExecutor,
        ISerialNumberGenerator serialNumberGenerator,
        ILogger<WalletRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbScriptExecutor = dbScriptExecutor ?? throw new ArgumentNullException(nameof(dbScriptExecutor));
        _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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
        var transactionId = await _serialNumberGenerator
            .GenerateAsync(cancellationToken)
            .ConfigureAwait(false);

        var updateResult = await Policy
            .Handle<PostgresException>(ex => ex.ErrorCode == 40001)
            .WaitAndRetryAsync(3, count =>
            {
                var wait = Random.Shared.Next(100, 1000);

                _logger.LogWarning("Exchange retry occur: {count}", count);
                return TimeSpan.FromMilliseconds(wait);
            })
            .ExecuteAsync(() => _dbScriptExecutor.QueryFirstOrDefaultAsync<WalletBalanceReturn>(
                @"
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
                        C.locked_balance AS PostLockedBalance",
                new
                {
                    walletId,
                    amount,
                    lockAmount,
                    updatedOn
                }))
            .ConfigureAwait(false);

        if (updateResult is null)
            throw new WalletExchangeFailException();

        if (updateResult.PostLockedBalance < 0M)
        {
            updateResult = await Policy
                .Handle<PostgresException>(ex => ex.ErrorCode == 40001)
                .WaitAndRetryAsync(3, count =>
                {
                    var wait = Random.Shared.Next(100, 1000);

                    _logger.LogWarning("Exchange retry occur: {count}", count);
                    return TimeSpan.FromMilliseconds(wait);
                })
                .ExecuteAsync(() => _dbScriptExecutor.QueryFirstOrDefaultAsync<WalletBalanceReturn>(
                    @"
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
                            C.locked_balance AS PostLockedBalance",
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

        _ = await _context.Transactions
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

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new ExchangeResult(
            transactionId,
            updateResult.PostBalance,
            updateResult.PostLockedBalance);
    }

    public async ValueTask<decimal?> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var playerWallet = await _context.WalletItems
            .Where(x => x.Id == walletId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return playerWallet?.Balance;
    }

    public async ValueTask<Wallet?> GetWalletAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var wallet = await (from w in _context.WalletItems.AsQueryable()
                            join p in _context.Players on w.PlayerCode equals p.PlayerCode
                            where w.Id == walletId
                            select new Wallet
                            {
                                Id = w.Id,
                                PlayerCode = w.PlayerCode,
                                BrandId = p.BrandId
                            }).FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return wallet;
    }
}
