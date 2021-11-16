using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record AdjustService : IGetService<AdjustCommand, ExchangeResult>
    {
        private readonly ILogger<AdjustService> _logger;

        private readonly IWalletRepository _walletRepository;

        public AdjustService(ILogger<AdjustService> logger, IWalletRepository walletRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<ExchangeResult> GetAsync(AdjustCommand query, CancellationToken cancellationToken = default)
        {
            var (transactionId, postBalance) = await _walletRepository.ExchangeAsync(
                query.BrandId,
                query.WalletId,
                query.PlayerCode,
                TransactionType.Adjust,
                query.Amount,
                query.Description,
                query.ExternalId,
                query.CreatedByUserId)
                .ConfigureAwait(false);

            return new ExchangeResult(transactionId, postBalance);
        }
    }
}
