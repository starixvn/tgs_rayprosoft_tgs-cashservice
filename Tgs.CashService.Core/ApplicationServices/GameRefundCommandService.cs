using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameRefundCommandService : IGetService<GameRefundCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public GameRefundCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public ValueTask<ExchangeResult> GetAsync(GameRefundCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.GameRefund.ToString(),
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
