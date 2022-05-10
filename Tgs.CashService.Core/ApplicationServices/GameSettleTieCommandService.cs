using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameSettleTieCommandService : IGetService<GameSettleTieCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public GameSettleTieCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public async ValueTask<ExchangeResult> GetAsync(
        GameSettleTieCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.GameTieBet.ToString(),
            Amount = command.Amount,
            LockAmount = 0M,
            Description = string.Empty,
            ExternalId = command.ExternalId
        };

        return await _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
