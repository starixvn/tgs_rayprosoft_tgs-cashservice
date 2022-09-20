using System.Text.Json;
using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustBonusCommandService : IGetService<AdjustBonusCommand, ExchangeResult>, IGetService<AdjustBonusV2Command, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public AdjustBonusCommandService(
        IWalletExchangeService walletExchangeService)
        => _walletExchangeService =
            walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public ValueTask<ExchangeResult> GetAsync(AdjustBonusCommand command,
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

        return _walletExchangeService
            .SendRequestAsync(request, cancellationToken);
    }

    public ValueTask<ExchangeResult> GetAsync(AdjustBonusV2Command command, CancellationToken cancellationToken = default)
    {
        var description = new
        {
            TransactionType = (int)TransactionType.AdjustBonus,
            Remark = command.Description
        };

        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.AdjustBonus.ToString(),
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
