using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class BonusCommandService : IGetService<BonusCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public BonusCommandService(
        IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public ValueTask<ExchangeResult> GetAsync(BonusCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.Bonus.ToString(),
            Amount = command.Amount,
            LockAmount = 0M,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        return _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
;
    }
}
