using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransactionTotalAmountQueryService : IGetService<TransactionTotalAmountQuery, decimal?>
{
    private readonly ITgsRepository _tgsRepository;

    public TransactionTotalAmountQueryService(ITgsRepository tgsRepository)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
    }

    public async ValueTask<decimal?> GetAsync(TransactionTotalAmountQuery query, CancellationToken cancellationToken = default)
    {
        var playerCode = await _tgsRepository
            .GetPlayerCodeByWalletIdAsync(query.WalletId, cancellationToken)
            .ConfigureAwait(false);

        return playerCode is null
            ? null
            : await _tgsRepository.QueryTransactionTotalAmountAsync(
                query.WalletId,
                playerCode,
                query.QueryRange,
                query.QueryTypes,
                cancellationToken);
    }
}
