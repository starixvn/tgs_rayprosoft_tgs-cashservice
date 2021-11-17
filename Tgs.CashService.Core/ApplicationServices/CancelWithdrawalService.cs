using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record CancelWithdrawalService : IGetService<CancelWithdrawalCommand, ExchangeResult>
    {
        private readonly ILogger<CancelWithdrawalService> _logger;

        private readonly IWalletRepository _walletRepository;

        public CancelWithdrawalService(ILogger<CancelWithdrawalService> logger, IWalletRepository walletRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
