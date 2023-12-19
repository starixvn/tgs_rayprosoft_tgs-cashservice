using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderFundOutCommandService(IWalletExchangeService walletExchangeService)
    : IGetService<GameProviderFundOutCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public ValueTask<ExchangeResult> GetAsync(GameProviderFundOutCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.GameProviderFundOut.ToString(),
            Amount = -command.Amount,
            LockAmount = 0M,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        return _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
;
    }
}
