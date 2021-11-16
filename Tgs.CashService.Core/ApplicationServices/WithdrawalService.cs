using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record WithdrawalService : IGetService<WithdrawalCommand, ExchangeResult>
    {
        private readonly ILogger<WithdrawalService> _logger;

        private readonly IWalletRepository _walletRepository;

        public WithdrawalService(ILogger<WithdrawalService> logger, IWalletRepository walletRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<ExchangeResult> GetAsync(WithdrawalCommand query, CancellationToken cancellationToken = default)
        {
            var wallet = await _walletRepository.GetWalletAsync(query.WalletId);

            if (wallet == null)
                throw new InvalidOperationException("Can't found wallet.");

            var (transactionId, postBalance) = await _walletRepository
                .ExchangeAsync(
                    wallet.BrandId,
                    query.WalletId,
                    wallet.PlayerCode,
                    TransactionType.Withdrawal,
                    query.Amount,
                    query.Description,
                    query.ExternalId)
                .ConfigureAwait(false);

            return new ExchangeResult(transactionId, postBalance);
        }
    }
}
