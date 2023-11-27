using Tgs.CashService;

namespace Tgs.CashServiceCommitter;

public interface ITgsRepository
{
    ValueTask InsertTransactionLogsAsync(IEnumerable<TransactionRecord> records, CancellationToken cancellationToken = default);

    ValueTask MigrateTransactionLogsAsync(IEnumerable<TransactionRecord> records, CancellationToken cancellationToken = default);

    ValueTask UpdateWalletAsync(Guid walletId, CancellationToken cancellationToken = default);
}
