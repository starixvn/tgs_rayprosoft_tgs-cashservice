using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices;

public class GetBalanceQueryService : IGetService<GetBalanceQuery, decimal>
{
    private readonly IWalletRepository _walletRepository;

    public GetBalanceQueryService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
    }

    public async ValueTask<decimal> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var balance = await _walletRepository
            .GetBalanceAsync(query.WalletId)
            .ConfigureAwait(false);

        return balance is null ? throw new InvalidOperationException("Can't found wallet.") : balance.Value;
    }
}
