using Tgs.ServiceUtility;

namespace Tgs.CashServiceWorker.ApplicationServices;

internal class GetBalanceQueryService(WalletExchanger walletExchanger)
    : IGetService<GetBalanceQuery, decimal>
{
    public async ValueTask<decimal> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var result = await walletExchanger.GetBalanceAsync(
            query.WalletId,
            cancellationToken).ConfigureAwait(false);

        return result.Balance;
    }
}
