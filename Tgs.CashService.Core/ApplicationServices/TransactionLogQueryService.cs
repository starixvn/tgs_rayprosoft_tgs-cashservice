using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransactionLogQueryService(ITgsRepository tgsRepository)
    : IQueryService<TransactionLogQuery, TransactionLog>
    , IGetService<TransactionLogQuery, int>
{
    public IAsyncEnumerable<TransactionLog> QueryAsync(TransactionLogQuery query, CancellationToken cancellationToken = default)
        => tgsRepository.QueryTransactionLogsAsync(
            query.WalletId,
            query.QueryRange,
            query.QueryTypes,
            query.Paging,
            cancellationToken);

    public async ValueTask<int> GetAsync(TransactionLogQuery query, CancellationToken cancellationToken = default)
        => await tgsRepository.CountTransactionLogsQueryAsync(
            query.WalletId,
            query.QueryRange,
            query.QueryTypes,
            cancellationToken).ConfigureAwait(false);
}
