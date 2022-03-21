using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameSettleWinCommandService : IGetService<GameSettleWinCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public GameSettleWinCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public async ValueTask<ExchangeResult> GetAsync(
        GameSettleWinCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.GameWinBet.ToString(),
            Amount = command.Amount,
            Description = string.Empty,
            ExternalId = command.ExternalId
        };

        return await _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
