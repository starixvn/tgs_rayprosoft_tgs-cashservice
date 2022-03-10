using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class CompanyAdjustCommandService : IGetService<CompanyAdjustCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public CompanyAdjustCommandService(
        IWalletExchangeService walletExchangeService)
        => _walletExchangeService =
            walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public async ValueTask<ExchangeResult> GetAsync(CompanyAdjustCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.CompanyAdjust.ToString(),
            Amount = command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty,
            CreatedUser = command.CreatedByUserId
        };

        return await _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}