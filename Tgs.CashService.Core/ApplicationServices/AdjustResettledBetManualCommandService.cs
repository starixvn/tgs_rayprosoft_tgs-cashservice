using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustResettledBetManualCommandService(IWalletExchangeService walletExchangeService)
    : IGetService<AdjustResettledBetManualCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public ValueTask<ExchangeResult> GetAsync(AdjustResettledBetManualCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.AdjustResettledBetManual.ToString(),
            Amount = command.Amount,
            LockAmount = 0M,
            Description = command.Description ?? String.Empty,
            ExternalId = command.ExternalId ?? String.Empty,
            CreatedUser = command.CreatedByUserId
        };

        return _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
;
    }
}
