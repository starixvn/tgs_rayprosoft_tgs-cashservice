using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.CashServiceWorker;
using TGS.GameProviders.Repositories.Entities;

namespace Tgs.CashService.Repositories;

internal class WalletRepository(
    TgsContext dbContext,
    IDatabase redisDatabase)
    : IWalletRepository
{
    private static readonly DateTime _MigrateDate = DateTime.Parse("2023-11-20").ToUniversalTime();

    private static readonly JsonSerializerOptions _SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = false,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

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

            if (player is null)
                return null;

            wallet.BrandId = player.BrandId;

            _ = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return (Wallet?)new Wallet(
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

    public async ValueTask<TransactionRecord?> GetNewestRecordAsync(Guid walletId, Guid? snapshotId, CancellationToken cancellationToken = default)
    {
        var key = new RedisKey($"Tgs:Transactions:{walletId}");

        var value = await redisDatabase.ListGetByIndexAsync(
            key,
            -1).ConfigureAwait(false);

        if (value.HasValue)
            return JsonSerializer.Deserialize<TransactionRecord>((string)value!, _SerializerOptions);

        var query = dbContext.Transactions
           .Where(row => row.WalletId == walletId
                && row.RecordedOn > _MigrateDate);

        query = snapshotId is not null
            ? query.Where(row => row.Id > snapshotId)
            : query.Where(row => row.Type == (short)TransactionType.Snapshot);

        var newestRecord = query
           .OrderByDescending(row => row.Id)
           .FirstOrDefault();

        return newestRecord is null
            ? null
            : new TransactionRecord(
                newestRecord.Id,
                newestRecord.WalletId,
                newestRecord.BrandId,
                newestRecord.PlayerCode,
                newestRecord.Amount,
                newestRecord.LockAmount,
                newestRecord.Balance,
                newestRecord.LockedBalance,
                (TransactionType)newestRecord.Type,
                newestRecord.Description,
                newestRecord.RecordedOn,
                newestRecord.ExternalTransactionId,
                newestRecord.CreatedByUserId);
    }
}
