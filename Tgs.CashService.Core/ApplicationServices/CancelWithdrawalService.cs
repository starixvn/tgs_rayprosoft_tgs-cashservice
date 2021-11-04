using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record CancelWithdrawalService : IGetService<CancelWithdrawalQuery, ExchangeResult>
    {
        private readonly ILogger<CancelWithdrawalService> _logger;

        private readonly IWalletRepository _walletRepository;

        public CancelWithdrawalService(ILogger<CancelWithdrawalService> logger, IWalletRepository walletRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<ExchangeResult> GetAsync(CancelWithdrawalQuery query, CancellationToken cancellationToken = default)
        {
            var (TransactionId, PostBalance) = await _walletRepository.ExchangeAsync(
                query.BrandId,
                query.WalletId,
                query.PlayerCode,
                TransactionTypes.CancelWithdrawal,
                query.Amount,
                query.Description,
                query.ExternalId,
                query.CreatedByUserId)
                .ConfigureAwait(false);
            return new()
            {
                TransactionId = TransactionId,
                Balance = PostBalance
            };
        }
    }
}
