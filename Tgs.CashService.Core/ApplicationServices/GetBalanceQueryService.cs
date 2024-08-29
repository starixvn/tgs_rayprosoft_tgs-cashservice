using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GetBalanceQueryService(ITgsRepository tgsRepository)
    : IGetService<GetBalanceQuery, decimal>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<decimal> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var wallet = await _tgsRepository.GetWalletByIdAsync(query.WalletId, cancellationToken).ConfigureAwait(false);

        return wallet is null ? throw new InvalidOperationException("Can't found wallet.") : wallet.Balance;
    }
}
