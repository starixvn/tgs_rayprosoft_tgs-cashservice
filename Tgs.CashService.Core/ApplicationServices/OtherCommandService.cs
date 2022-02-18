using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class OtherCommandService : IGetService<OtherCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public OtherCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public async ValueTask<ExchangeResult> GetAsync(OtherCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.Other.ToString(),
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
