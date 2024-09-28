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
        try
        {
            var errorCode = IsValidData(command, cancellationToken);
            if (errorCode != ErrorCode.Success)
            {
                logger.LogError($"Adjust Balance - {ErrorMessages.Messages[errorCode]} {JsonSerializer.Serialize(command)}");
                return new BalanceResult(Guid.Empty, 0, 0, errorCode);
            }

            var record = new ExchangeWalletRecord(
            null,
            TransactionType.GameProviderAdjustBalance,
            command.AddedAmount - command.DeductedAmount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty,
            null,
            command.PlayerCode,
            true,
            command.ProviderCode,
            command.OperationId,
            command.Action
            );

            var result = await _tgsRepository.ExchangeWalletAsync(record, cancellationToken)
                .ConfigureAwait(false);
            return new BalanceResult(result.TransactionId, result.Balance, result.LockedBalance);
        }
        catch (Exception ex) {
            logger.LogError($"Adjust Balance - {ex.Message} - {JsonSerializer.Serialize(command)}");
            var code = ExceptionMapper.GetErrorCodeByException(ex);
            return new BalanceResult(Guid.Empty, 0, 0, code);
        }
    }

    private string IsValidData(GameProviderAdjustBalanceCommand command, CancellationToken cancellationToken = default)
    {
        if (command.AddedAmount < 0 || command.DeductedAmount < 0)
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
