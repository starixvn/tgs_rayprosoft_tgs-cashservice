using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderDeductBalanceCommandService(ITgsRepository tgsRepository,
    ILogger<GameProviderDeductBalanceCommand> logger)
    : IGetService<GameProviderDeductBalanceCommand, BalanceResult>
{
    public async ValueTask<BalanceResult> GetAsync(GameProviderDeductBalanceCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var errorCode = IsValidData(command);
            if (errorCode != ErrorCode.Success)
            {
                logger.LogError($"Deduct Balance - {ErrorMessageMapper.Messages[errorCode]} {JsonSerializer.Serialize(command)}");
                return new BalanceResult(Guid.Empty, 0, 0, errorCode);
            }

            var record = new ExchangeWalletRecord(
            null,
            TransactionType.GameProviderDeductBalance,
            -command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty,
            null,
            command.PlayerCode,
            false,
            command.ProviderCode,
            command.OperationId,
            command.Action
            );

            var result = await tgsRepository.ExchangeWalletAsync(record, cancellationToken)
                .ConfigureAwait(false);
            return new BalanceResult(result.TransactionId, result.Balance, result.LockedBalance);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Deduct Balance - {ex.Message} - {JsonSerializer.Serialize(command)}");
            var code = ExceptionMapper.GetErrorCodeByException(ex);
            return new BalanceResult(Guid.Empty, 0, 0, code);
        }
    }

    private string IsValidData(GameProviderDeductBalanceCommand command)
    {
        if (command.Amount <= 0)
        {
            return ErrorCode.InvalidAmount;
        }
        if (string.IsNullOrEmpty(command.ExternalId) || string.IsNullOrEmpty(command.ProviderCode)
            || string.IsNullOrEmpty(command.OperationId) || string.IsNullOrEmpty(command.Action))
        {
            return ErrorCode.InvalidData;
        }
        return ErrorCode.Success;
    }
}
