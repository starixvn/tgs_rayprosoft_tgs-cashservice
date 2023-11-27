using Tgs.CashService;

namespace Tgs.CashServiceCommitter;

public interface ITransactionLogRepository
{
    IAsyncEnumerable<TransactionRecord> GetWaitingBookkeepLogsAsync(Guid walletId, CancellationToken cancellationToken = default);

    ValueTask RemoveBookkeepLogsAsync(Guid walletId, int length, CancellationToken cancellationToken = default);
}
