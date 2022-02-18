using Tgs.ServiceUtility;

namespace Tgs.CashServiceWorker.ApplicationServices;

internal class GetBalanceQueryService : IGetService<GetBalanceQuery, decimal>
{
    private readonly IWalletRepository _walletRepository;

    public GetBalanceQueryService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
    }

    public async ValueTask<decimal> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
        => await _walletRepository
            .GetBalanceAsync(query.WalletId)
            .ConfigureAwait(false) ?? throw new InvalidOperationException("Can't found wallet.");
}
