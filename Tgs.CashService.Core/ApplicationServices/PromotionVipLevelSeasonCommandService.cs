using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class PromotionVipLevelSeasonCommandService(IWalletExchangeService walletExchangeService)
    : IGetService<PromotionVipLevelSeasonCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public async ValueTask<ExchangeResult> GetAsync(PromotionVipLevelSeasonCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.PromotionVipLevelSeason.ToString(),
            Amount = command.Amount,
            LockAmount = 0M,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        return await _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
