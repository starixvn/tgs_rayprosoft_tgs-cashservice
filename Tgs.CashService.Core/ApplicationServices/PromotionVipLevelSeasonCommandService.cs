using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices
{
    public class PromotionVipLevelSeasonCommandService : IGetService<PromotionVipLevelSeasonCommand, ExchangeResult>
    {
        private readonly IWalletRepository _walletRepository;

        public PromotionVipLevelSeasonCommandService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<ExchangeResult> GetAsync(PromotionVipLevelSeasonCommand query, CancellationToken cancellationToken = default)
        {
            var wallet = await _walletRepository.GetWalletAsync(query.WalletId);

            if (wallet == null)
                throw new InvalidOperationException("Can't found wallet.");

            var (transactionId, postBalance) = await _walletRepository
                .ExchangeAsync(
                    wallet.BrandId,
                    query.WalletId,
                    wallet.PlayerCode,
                    TransactionType.PromotionVipLevelSeason,
                    query.Amount,
                    query.Description,
                    query.ExternalId)
                .ConfigureAwait(false);

            return new ExchangeResult(transactionId, postBalance);
        }
    }
}
