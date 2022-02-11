using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices;

internal class AdjustResettledBetManualCommandService : IGetService<AdjustResettledBetManualCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public AdjustResettledBetManualCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public async ValueTask<ExchangeResult> GetAsync(AdjustResettledBetManualCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.AdjustResettledBetManual.ToString(),
            Amount = command.Amount,
            Description = command.Description ?? String.Empty,
            ExternalId = command.ExternalId ?? String.Empty,
            CreatedUser = command.CreatedByUserId
        };

        return await _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
