using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustBonusCommandService : IGetService<AdjustBonusCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public AdjustBonusCommandService(
        IWalletExchangeService walletExchangeService)
        => _walletExchangeService =
            walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public async ValueTask<ExchangeResult> GetAsync(AdjustBonusCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.AdjustBonus.ToString(),
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
