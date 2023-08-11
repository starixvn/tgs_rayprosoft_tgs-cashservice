using Tgs.Cash.Common;
using Tgs.Paging;

namespace Tgs.CashService;

public interface ITgsRepository
{
    ValueTask<string?> GetPlayerCodeByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TransactionLog> QueryTransactionLogsAsync(
        Guid walletId,
        string playerCode,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        Paging<TransactionLog> paging,
        CancellationToken cancellationToken = default);

    ValueTask<int> CountTransactionLogsQueryAsync(
        Guid walletId,
        string playerCode,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default);
}
