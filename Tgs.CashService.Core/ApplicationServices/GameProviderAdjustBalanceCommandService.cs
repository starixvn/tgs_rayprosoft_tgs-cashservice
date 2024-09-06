using Microsoft.Extensions.Logging;
using System.Text.Json;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderAdjustBalanceCommandService(ITgsRepository tgsRepository,
    ILogger<GameProviderAdjustBalanceCommand> logger)
    : IGetService<GameProviderAdjustBalanceCommand, BalanceResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<BalanceResult> GetAsync(GameProviderAdjustBalanceCommand command, CancellationToken cancellationToken = default)
    {
        if(!await IsValidTransaction(command.ExternalId, cancellationToken).ConfigureAwait(false))
        {
            logger.LogError($"Adjust Balance - Duplicate transaction {JsonSerializer.Serialize(command)}");
            return new BalanceResult(Guid.Empty, 0, 0, ErrorCode.DuplicateTransaction);
        }
        try
        {
            var record = new ExchangeWalletRecord(
            null,
            TransactionType.GameProviderAdjustBalance,
            command.AddedAmount - command.DeductedAmount,
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
        return new BalanceResult(Guid.Empty, 0, 0, ErrorCode.NotFoundWallet);
    }

    private async ValueTask<bool> IsValidTransaction(string? externalId, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(externalId))
        {
            var transaction = await _tgsRepository
                .GetTransactionLogByTypeAndExternalIdAsync(TransactionType.GameProviderAdjustBalance, externalId, cancellationToken)
                .ConfigureAwait(false);
            return transaction == null;
        }
        return true;
    }
}
