using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderSettleLoseCommandService : IGetService<GameProviderSettleLoseCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public GameProviderSettleLoseCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public async ValueTask<ExchangeResult> GetAsync(
        GameProviderSettleLoseCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.LoseBet.ToString(),
            Amount = -command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId
        };

        return await _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
