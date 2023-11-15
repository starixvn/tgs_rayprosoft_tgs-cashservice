using Tgs.ServiceUtility;

namespace Tgs.CashServiceWorker.ApplicationServices;

internal class GetBalanceQueryService(IWalletRepository walletRepository)
    : IGetService<GetBalanceQuery, decimal>
{
    private readonly IWalletRepository _walletRepository = walletRepository
        ?? throw new ArgumentNullException(nameof(walletRepository));

    public async ValueTask<decimal> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
        => await _walletRepository
            .GetBalanceAsync(query.WalletId, cancellationToken)
            .ConfigureAwait(false) ?? throw new InvalidOperationException("Can't found wallet.");
}
