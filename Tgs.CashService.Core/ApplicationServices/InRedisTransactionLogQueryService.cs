using Tgs.Paging;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class InRedisTransactionLogQueryService(
    ITgsRepository tgsRepository)
    : IQueryService<InRedisTransactionLogQuery, TransactionLog>
    , IGetService<InRedisTransactionLogQuery, int>
{
    public IAsyncEnumerable<TransactionLog> QueryAsync(InRedisTransactionLogQuery query, CancellationToken cancellationToken = default)
        => tgsRepository.QueryInRedisTransactionLogsAsync(
            query.WalletId,
            cancellationToken)
            .ApplyPaging(query.Paging);

    public ValueTask<int> GetAsync(InRedisTransactionLogQuery query, CancellationToken cancellationToken = default)
        => tgsRepository.CountInRedisTransactionLogsAsync(query.WalletId, cancellationToken);
}
