using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record CancelWithdrawalService : IGetService<CancelWithdrawalCommand, ExchangeResult>
    {
        private readonly IWalletRepository _walletRepository;

        public CancelWithdrawalService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<ExchangeResult> GetAsync(CancelWithdrawalCommand command, CancellationToken cancellationToken = default)
        {
            var wallet = await _walletRepository
                .GetWalletAsync(command.WalletId)
                .ConfigureAwait(false);

            if (wallet == null)
                throw new InvalidOperationException("Can't found wallet.");

            var (transactionId, postBalance) = await _walletRepository
                .ExchangeAsync(
                    wallet.BrandId,
                    command.WalletId,
                    wallet.PlayerCode,
                    TransactionType.CancelWithdrawal,
                    command.Amount,
                    command.Description,
                    command.ExternalId,
                    command.CreatedByUserId)
                .ConfigureAwait(false);

            return new ExchangeResult(transactionId, postBalance);
        }
    }
}
