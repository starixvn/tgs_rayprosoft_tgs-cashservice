using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderDeductBalanceCommandService(IWalletExchangeService walletExchangeService, 
    ITgsRepository tgsRepository,
    ILogger<GameProviderDeductBalanceCommand> logger)
    : IGetService<GameProviderDeductBalanceCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<ExchangeResult> GetAsync(GameProviderDeductBalanceCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            logger.LogError($"Deduct Invalid Amount {JsonSerializer.Serialize(command)}");
            throw new Exception("Deduct Invalid Amount");
        }
        Guid walletId = await tgsRepository.GetWalletIdAsync(command.PlayerCode, command.WalletId, cancellationToken).ConfigureAwait(false);
        var request = new WalletExchangeRequest
        {
            WalletId = walletId,
            TransactionType = TransactionType.GameProviderDeductBalance.ToString(),
            Amount = command.Amount,
            LockAmount = 0M,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        var response = await _walletExchangeService
            .SendRequestAsync(request, cancellationToken).ConfigureAwait(false);

        // TODO: need to return response has error code
        if(response.Balance < 0)
        {
            logger.LogError($"Not enough balance to deduct {JsonSerializer.Serialize(command)}");
            throw new Exception("Not enough balance to deduct");
        }
        return response;
    }
}
