using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransactionLogQueryService(ITgsRepository tgsRepository)
    : IGetService<TransactionLogQuery, IEnumerable<TransactionLog>>
{
    public async ValueTask<IEnumerable<TransactionLog>> GetAsync(TransactionLogQuery query, CancellationToken cancellationToken = default)
        => await tgsRepository.QueryTransactionLogsAsync(
            query.WalletId,
            query.QueryRange,
            query.QueryTypes,
            query.PageIndex,
            query.PageSize,
            cancellationToken).ConfigureAwait(false);
}
