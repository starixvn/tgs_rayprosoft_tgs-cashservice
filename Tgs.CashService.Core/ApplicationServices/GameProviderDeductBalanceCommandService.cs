using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderDeductBalanceCommandService(IWalletExchangeService walletExchangeService, 
    ITgsRepository tgsRepository,
    ILogger<GameProviderDeductBalanceCommand> logger)
    : IGetService<GameProviderDeductBalanceCommand, BalanceResult>
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<BalanceResult> GetAsync(GameProviderDeductBalanceCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            logger.LogError($"Deduct Invalid Amount {JsonSerializer.Serialize(command)}");
            return new BalanceResult(Guid.Empty, 0, 0, 1, "Invalid Amount");
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
            return new BalanceResult(response.TransactionId, response.Balance, response.LockedBalance, 1, "Not enough balance to deduct");
        }
        return new BalanceResult(response.TransactionId, response.Balance, response.LockedBalance, 0, string.Empty);

    }
}
