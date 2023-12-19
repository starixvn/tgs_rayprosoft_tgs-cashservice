using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tgs.CashService;
using Tgs.CashServiceCommitter.Repositories.Entities;

namespace Tgs.CashServiceCommitter.Repositories;

internal class TgsRepository(
    TgsContext dbContext,
    ILogger<TgsRepository> logger) : ITgsRepository
{
    private static readonly DateTime _MigrateDate = DateTime.Parse("2023-11-20").ToUniversalTime();

    public async ValueTask InsertTransactionLogsAsync(IEnumerable<TransactionRecord> records, CancellationToken cancellationToken = default)
    {
        await dbContext.Transactions.AddRangeAsync(records.Select(record => new Transaction
        {
            Id = record.Id,
            BrandId = record.BrandId,
            PlayerCode = record.UserId,
            WalletId = record.WalletId,
            Type = (short)record.Type,
            Amount = record.Amount,
            LockAmount = record.LockAmount,
            Balance = record.Balance,
            LockedBalance = record.LockedBalance,
            Description = record.Description,
            ExternalTransactionId = record.ExternalId,
            CreatedByUserId = record.Modifier,
            RecordedOn = record.RecordedOn.ToUniversalTime(),
            SentOn = null
        }), cancellationToken).ConfigureAwait(false);

        _ = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask MigrateTransactionLogsAsync(IEnumerable<TransactionRecord> records, CancellationToken cancellationToken = default)
    {
        dbContext.ChangeTracker.Clear();

        var recordIds = records.Select(record => record.Id).ToList();
        var alreadyInsertedIds = await dbContext.Transactions
            .Where(row => recordIds.Contains(row.Id))
            .Select(row => row.Id)
            .ToArrayAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var insertingRecords = records.Where(
            record => !alreadyInsertedIds.Contains(record.Id)).ToArray();

        if (insertingRecords is null || insertingRecords.Length == 0)
            return;

        await InsertTransactionLogsAsync(
            insertingRecords,
            cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateWalletAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var walletRow = await dbContext.Wallets.FindAsync(
            [walletId],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Find walletRow is {walletRow}", walletRow);

        if (walletRow is null)
            return;

        var newestRecord = dbContext.Transactions
            .Where(row => row.WalletId == walletId
                && row.RecordedOn > _MigrateDate
                && (walletRow.SnapshotId == null && row.Type == (int)TransactionType.Snapshot || row.Id > walletRow.SnapshotId))
            .OrderByDescending(row => row.Id)
            .FirstOrDefault();

        logger.LogDebug("Newest record is {newestRecord}", newestRecord);

        if (newestRecord is null)
            return;

        _ = await dbContext.Wallets
            .Where(row => row.Id == walletId && (row.SnapshotId == null || row.SnapshotId < newestRecord.Id))
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(row => row.Balance, newestRecord.Balance)
                    .SetProperty(row => row.LockedBalance, newestRecord.LockedBalance)
                    .SetProperty(row => row.SnapshotId, newestRecord.Id)
                    .SetProperty(row => row.UpdatedOn, newestRecord.RecordedOn),
                cancellationToken).ConfigureAwait(false);

        _ = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
