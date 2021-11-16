using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record GameProviderDepositService : IGetService<GameProviderDepositCommand, ExchangeResult>
    {
        private readonly ILogger<GameProviderDepositService> _logger;

        private readonly IWalletRepository _walletRepository;

        public GameProviderDepositService(ILogger<GameProviderDepositService> logger, IWalletRepository walletRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<ExchangeResult> GetAsync(GameProviderDepositCommand query, CancellationToken cancellationToken = default)
        {
            var (TransactionId, PostBalance) = await _walletRepository.ExchangeAsync(
                query.BrandId,
                query.WalletId,
                query.PlayerCode,
                TransactionTypes.GameProviderDeposit,
                query.Amount,
                query.Description,
                query.ExternalId)
                .ConfigureAwait(false);
            return new()
            {
                TransactionId = TransactionId,
                Balance = PostBalance
            };
        }
    }
}
