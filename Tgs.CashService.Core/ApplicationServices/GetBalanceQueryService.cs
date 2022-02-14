using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices;

internal class GetBalanceQueryService : IGetService<GetBalanceQuery, decimal>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public GetBalanceQueryService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public async ValueTask<decimal> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var balance = await _walletExchangeService
            .GetBalanceAsync(query.WalletId)
            .ConfigureAwait(false);

        return balance is null ? throw new InvalidOperationException("Can't found wallet.") : balance.Value;
    }
}
