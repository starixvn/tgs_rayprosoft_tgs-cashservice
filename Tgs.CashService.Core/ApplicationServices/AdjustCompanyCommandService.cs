using System.Text.Json;
using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustCompanyCommandService(
    IWalletExchangeService walletExchangeService)
    : IGetService<AdjustCompanyV2Command, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService 
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    public ValueTask<ExchangeResult> GetAsync(AdjustCompanyV2Command command, CancellationToken cancellationToken = default)
    {
        var description = new
        {
            TransactionType = (int)TransactionType.AdjustCompany,
            Remark = command.Description
        };

        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.AdjustCompany.ToString(),
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
