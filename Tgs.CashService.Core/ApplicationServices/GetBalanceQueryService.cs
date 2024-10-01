using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GetBalanceQueryService(ITgsRepository tgsRepository)
    : IGetService<GetBalanceQuery, decimal>
{
    public async ValueTask<decimal> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var wallet = await tgsRepository.GetWalletByIdAsync(query.WalletId, cancellationToken).ConfigureAwait(false);

        return wallet is null ? throw new InvalidOperationException("Can't found wallet.") : wallet.Balance;
    }
}
