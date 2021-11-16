using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record GameProviderFundInService : IGetService<GameProviderFundInCommand, ExchangeResult>
    {
        private readonly ILogger<GameProviderFundInService> _logger;

        private readonly IWalletRepository _walletRepository;

        public GameProviderFundInService(ILogger<GameProviderFundInService> logger, IWalletRepository walletRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<ExchangeResult> GetAsync(GameProviderFundInCommand query, CancellationToken cancellationToken = default)
        {
            var (transactionId, postBalance) = await _walletRepository.ExchangeAsync(
                query.BrandId,
                query.WalletId,
                query.PlayerCode,
                TransactionType.GameProviderFundIn,
                query.Amount,
                query.Description,
                query.ExternalId)
                .ConfigureAwait(false);

            return new ExchangeResult(transactionId, postBalance);
        }
    }
}
