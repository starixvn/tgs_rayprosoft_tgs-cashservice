using Tgs.Cash.Common;
using Tgs.Paging;

namespace Tgs.CashService;

public interface ITgsRepository
{
    ValueTask<int> CountTransactionLogsQueryAsync(
        Guid walletId,
        string playerCode,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default);

    ValueTask CreateWalletAsync(
        Guid walletId,
        Guid brandId,
        string userId,
        CashType type,
        string currencyCode,
        CancellationToken cancellationToken = default);

    ValueTask<string?> GetPlayerCodeByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TransactionLog> QueryTransactionLogsAsync(
        Guid walletId,
        string playerCode,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        Paging<TransactionLog> paging,
        CancellationToken cancellationToken = default);

    ValueTask<decimal> QueryTransactionTotalAmountAsync(
        Guid walletId,
        string playerCode,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default);
}
