using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameBetCommandService : IGetService<GameBetCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public GameBetCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public ValueTask<ExchangeResult> GetAsync(GameBetCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.GameBet.ToString(),
            Amount = -command.Amount,
            LockAmount = 0M,
            Description = string.Empty,
            ExternalId = command.ExternalId
        };

        return _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
;
    }
}
