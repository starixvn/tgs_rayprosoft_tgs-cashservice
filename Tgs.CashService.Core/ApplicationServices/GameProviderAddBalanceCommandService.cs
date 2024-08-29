using Microsoft.Extensions.Logging;
using System.Text.Json;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderAddBalanceCommandService(ITgsRepository tgsRepository,
    ILogger<GameProviderDeductBalanceCommand> logger)
    : IGetService<GameProviderAddBalanceCommand, BalanceResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<BalanceResult> GetAsync(GameProviderAddBalanceCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            logger.LogError($"Invalid Amount {JsonSerializer.Serialize(command)}");
            return new BalanceResult(Guid.Empty, 0, 0, ErrorCodes.InvalidAmount);
        }
        try
        {
            var record = new ExchangeWalletRecord(
            null,
            TransactionType.GameProviderAddBalance,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty,
            null,
            command.PlayerCode
            );

            var result = await _tgsRepository.ExchangeWalletAsync(record, cancellationToken)
                .ConfigureAwait(false);
            return new BalanceResult(result.TransactionId, result.Balance, result.LockedBalance);
        }
        catch (Exception ex) {
            logger.LogError(ex.Message);
        }
        return new BalanceResult(Guid.Empty, 0, 0, ErrorCodes.NotFoundWallet);
    }
}
