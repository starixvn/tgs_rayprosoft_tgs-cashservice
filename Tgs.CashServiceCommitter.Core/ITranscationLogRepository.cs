using Tgs.CashService;

namespace Tgs.CashServiceCommitter;

public interface ITranscationLogRepository
{
    IAsyncEnumerable<TransactionRecord> GetWaitingBookkeepLogsAsync(Guid walletId, CancellationToken cancellationToken = default);

    ValueTask RemoveBookkeepLogsAsync(Guid walletId, int length, CancellationToken cancellationToken = default);
}
