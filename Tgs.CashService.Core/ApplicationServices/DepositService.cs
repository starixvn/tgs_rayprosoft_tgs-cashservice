using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record DepositService : IGetService<DepositCommand, ExchangeResult>
    {
        private readonly IWalletRepository _walletRepository;

        public DepositService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<ExchangeResult> GetAsync(DepositCommand query, CancellationToken cancellationToken = default)
        {
            var wallet = await _walletRepository.GetWalletAsync(query.WalletId);

            if (wallet == null)
                throw new InvalidOperationException("Can't found wallet.");

            var (transactionId, postBalance) = await _walletRepository.ExchangeAsync(
                wallet.BrandId,
                query.WalletId,
                wallet.PlayerCode,
                TransactionType.Deposit,
                query.Amount,
                query.Description,
                query.ExternalId)
                .ConfigureAwait(false);

            return new ExchangeResult(transactionId, postBalance);
        }
    }
}
