using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransactionLogQueryService(ITgsRepository tgsRepository)
    : IQueryService<TransactionLogQuery, TransactionLog>
    , IGetService<TransactionLogQuery, int>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public IAsyncEnumerable<TransactionLog> QueryAsync(TransactionLogQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.QueryTransactionLogsAsync(
            query.WalletId,
            query.QueryRange,
            query.QueryTypes,
            query.Paging,
            cancellationToken);

    public async ValueTask<int> GetAsync(TransactionLogQuery query, CancellationToken cancellationToken = default)
        => await _tgsRepository.CountTransactionLogsQueryAsync(
            query.WalletId,
            query.QueryRange,
            query.QueryTypes,
            cancellationToken).ConfigureAwait(false);
}
