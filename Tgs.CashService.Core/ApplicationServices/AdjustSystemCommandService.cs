using System.Text.Json;
using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustSystemCommandService : IGetService<AdjustSystemV2Command, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public AdjustSystemCommandService(IWalletExchangeService walletExchangeService) => _walletExchangeService =
        walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public ValueTask<ExchangeResult> GetAsync(AdjustSystemV2Command command, CancellationToken cancellationToken = default)
    {
        var description = new
        {
            TransactionType = (int)TransactionType.AdjustSystem,
            Remark = command.Description
        };

        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.AdjustSystem.ToString(),
            Amount = command.Amount,
            LockAmount = command.LockAmount,
            Description = JsonSerializer.Serialize(description),
            ExternalId = command.ExternalId ?? string.Empty,
            CreatedUser = command.CreatedByUserId
        };

        return _walletExchangeService
            .SendRequestAsync(request, cancellationToken);
    }
}
