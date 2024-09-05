using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderDeductBalanceCommandService(ITgsRepository tgsRepository,
    ILogger<GameProviderDeductBalanceCommand> logger)
    : IGetService<GameProviderDeductBalanceCommand, BalanceResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<BalanceResult> GetAsync(GameProviderDeductBalanceCommand command, CancellationToken cancellationToken = default)
    {
        if (!await IsValidTransaction(command.ExternalId, cancellationToken).ConfigureAwait(false))
        {
            logger.LogError($"Deduct Balance - Duplicate transaction {JsonSerializer.Serialize(command)}");
            return new BalanceResult(Guid.Empty, 0, 0, ErrorCodes.DuplicateTransaction);
        }
        if (command.Amount <= 0)
        {
            logger.LogError($"Deduct Balance - Invalid Amount {JsonSerializer.Serialize(command)}");
            return new BalanceResult(Guid.Empty, 0, 0, ErrorCodes.InvalidAmount);
        }
        try
        {
            var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.GameProviderDeductBalance,
            -command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty,
            null,
            command.PlayerCode,
            false
            );

            var result = await _tgsRepository.ExchangeWalletAsync(record, cancellationToken)
                .ConfigureAwait(false);
            return new BalanceResult(result.TransactionId, result.Balance, result.LockedBalance);
        }
        catch (Exception ex) {
            logger.LogError(ex.Message);
            var errorCode = ex is NotFoundWalletException ? ErrorCodes.NotFoundWallet : ErrorCodes.InsufficientFunds;
            return new BalanceResult(Guid.Empty, 0, 0, errorCode);
        }
    }

    private async ValueTask<bool> IsValidTransaction(string? externalId, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(externalId))
        {
            var transaction = await _tgsRepository
                .GetTransactionLogByTypeAndExternalIdAsync(TransactionType.GameProviderDeductBalance, externalId, cancellationToken)
                .ConfigureAwait(false);
            return transaction == null;
        }
        return true;
    }
}
