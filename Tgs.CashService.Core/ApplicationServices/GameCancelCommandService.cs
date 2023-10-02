using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameCancelCommandService : IGetService<GameCancelCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public GameCancelCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public ValueTask<ExchangeResult> GetAsync(GameCancelCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.GameCancel.ToString(),
            Amount = command.Amount,
            LockAmount = 0M,
            Description = string.Empty,
            ExternalId = command.ExternalId
        };

        return _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
;
    }
}
