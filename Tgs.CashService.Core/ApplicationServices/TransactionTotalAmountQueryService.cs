using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransactionTotalAmountQueryService(ITgsRepository tgsRepository) : IGetService<TransactionTotalAmountQuery, decimal>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public ValueTask<decimal> GetAsync(TransactionTotalAmountQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.QueryTransactionTotalAmountAsync(
            query.WalletId,
            query.QueryRange,
            query.QueryTypes,
            cancellationToken);
}
