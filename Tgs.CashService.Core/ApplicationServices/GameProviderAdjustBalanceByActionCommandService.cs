using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderAdjustBalanceByActionCommandService(ITgsRepository tgsRepository,
    ILogger<GameProviderAdjustBalanceByActionCommand> logger)
    : IGetService<GameProviderAdjustBalanceByActionCommand, BalanceResult>
{
    public async ValueTask<BalanceResult> GetAsync(GameProviderAdjustBalanceByActionCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var errorCode = IsValidData(command);
            if (errorCode != ErrorCode.Success)
            {
                logger.LogError($"Adjust Balance With Action - {ErrorMessageMapper.Messages[errorCode]} {JsonSerializer.Serialize(command)}");
                return new BalanceResult(Guid.Empty, 0, 0, errorCode);
            }

            var record = new ExchangeWalletRecord(
            null,
            TransactionType.GameProviderDeductBalance,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty,
            null,
            command.PlayerCode,
            command.IsAllowNegative,
            command.ProviderCode,
            command.OperationId,
            command.Action
            );

            var result = await tgsRepository.ExchangeWalletByActionAsync(record, cancellationToken)
                .ConfigureAwait(false);
            return new BalanceResult(result.TransactionId, result.Balance, result.LockedBalance);
        }
        catch (Exception ex)
        {
            logger.LogError($"Adjust Balance With Action - {ex.Message} - {JsonSerializer.Serialize(command)}");
            var code = ExceptionMapper.GetErrorCodeByException(ex);
            return new BalanceResult(Guid.Empty, 0, 0, code);
        }
    }

    private string IsValidData(GameProviderAdjustBalanceByActionCommand command)
    {
        if (string.IsNullOrEmpty(command.ExternalId) || string.IsNullOrEmpty(command.ProviderCode)
            || string.IsNullOrEmpty(command.OperationId) || string.IsNullOrEmpty(command.Action))
        {
            return ErrorCode.InvalidData;
        }
        return ErrorCode.Success;
    }
}
