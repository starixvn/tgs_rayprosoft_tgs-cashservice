using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GetBalanceQueryService(IWalletExchangeService walletExchangeService)
    : IGetService<GetBalanceQuery, decimal>
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public async ValueTask<decimal> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var balance = await _walletExchangeService
            .GetBalanceAsync(query.WalletId, cancellationToken)
            .ConfigureAwait(false);

        return balance is null ? throw new InvalidOperationException("Can't found wallet.") : balance.Value;
    }
}
