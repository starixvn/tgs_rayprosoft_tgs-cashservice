using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices
{
    public class BalanceService : IGetService<GetBalanceQuery, decimal>
    {
        private readonly IWalletRepository _walletRepository;

        public BalanceService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<decimal> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var balance = await _walletRepository
                .GetBalanceAsync(query.WalletId)
                .ConfigureAwait(false);

            if (balance is null)
                throw new InvalidOperationException("Can't found wallet.");

            return balance.Value;
        }
    }
}
