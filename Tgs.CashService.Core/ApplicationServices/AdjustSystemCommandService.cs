using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustSystemCommandService : IGetService<AdjustSystemCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public AdjustSystemCommandService(IWalletExchangeService walletExchangeService) => _walletExchangeService =
        walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public async ValueTask<ExchangeResult> GetAsync(AdjustSystemCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.AdjustSystem.ToString(),
            Amount = command.Amount,
            LockAmount = command.LockAmount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty,
            CreatedUser = command.CreatedByUserId
        };

        return await _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
