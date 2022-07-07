using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustTransferCommandService : IGetService<AdjustTransferCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public AdjustTransferCommandService(IWalletExchangeService walletExchangeService) => _walletExchangeService =
        walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public async ValueTask<ExchangeResult> GetAsync(AdjustTransferCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.AdjustTransfer.ToString(),
            Amount = command.Amount,
            LockAmount = 0M,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty,
            CreatedUser = command.CreatedByUserId
        };

        return await _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
