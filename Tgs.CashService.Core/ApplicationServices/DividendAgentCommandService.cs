using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class DividendAgentCommandService : IGetService<DividendAgentCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public DividendAgentCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public ValueTask<ExchangeResult> GetAsync(DividendAgentCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = nameof(TransactionType.AgentDividend),
            Amount = command.Amount.Truncate(2),
            LockAmount = 0M,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        return _walletExchangeService
            .SendRequestAsync(request, cancellationToken);
    }
}
