using System.Text.Json;
using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustAgentCommandService : IGetService<AdjustAgentCommand, ExchangeResult>, IGetService<AdjustAgentV2Command, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public AdjustAgentCommandService(IWalletExchangeService walletExchangeService) => _walletExchangeService =
        walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public ValueTask<ExchangeResult> GetAsync(AdjustAgentCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.AdjustAgent.ToString(),
            Amount = command.Amount,
            LockAmount = 0M,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty,
            CreatedUser = command.CreatedByUserId
        };

        return _walletExchangeService
            .SendRequestAsync(request, cancellationToken);
    }

    public ValueTask<ExchangeResult> GetAsync(AdjustAgentV2Command command, CancellationToken cancellationToken = default)
    {
        var description = new
        {
            TransactionType = (int)TransactionType.AdjustAgent,
            Remark = command.Description
        };

        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.AdjustAgent.ToString(),
            Amount = command.Amount,
            LockAmount = 0M,
            Description = JsonSerializer.Serialize(description),
            ExternalId = command.ExternalId ?? string.Empty,
            CreatedUser = command.CreatedByUserId
        };

        return _walletExchangeService
            .SendRequestAsync(request, cancellationToken);
    }
}
