using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransactionTotalAmountQueryService(ITgsRepository tgsRepository) : IGetService<TransactionTotalAmountQuery, decimal>
{
    public ValueTask<decimal> GetAsync(TransactionTotalAmountQuery query, CancellationToken cancellationToken = default)
        => tgsRepository.QueryTransactionTotalAmountAsync(
            query.WalletId,
            query.QueryRange,
            query.QueryTypes,
            cancellationToken);
}
