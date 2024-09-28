using Microsoft.Extensions.Logging;
using System.Text.Json;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderAddBalanceCommandService(ITgsRepository tgsRepository,
    ILogger<GameProviderAddBalanceCommand> logger)
    : IGetService<GameProviderAddBalanceCommand, BalanceResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<BalanceResult> GetAsync(GameProviderAddBalanceCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var errorCode = IsValidData(command, cancellationToken);
            if (errorCode != ErrorCode.Success)
            {
                logger.LogError($"Add Balance - {ErrorMessages.Messages[errorCode]} {JsonSerializer.Serialize(command)}");
                return new BalanceResult(Guid.Empty, 0, 0, errorCode);
            }

            var record = new ExchangeWalletRecord(
            null,
            TransactionType.GameProviderAddBalance,
            command.Amount,
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
            logger.LogError($"Add Balance - {ex.Message} - {JsonSerializer.Serialize(command)}");
            var code = ExceptionMapper.GetErrorCodeByException(ex);
            return new BalanceResult(Guid.Empty, 0, 0, code);
        }
    }

    private string IsValidData(GameProviderAddBalanceCommand command, CancellationToken cancellationToken = default)
    {
        if(command.Amount <= 0)
        {
            return ErrorCode.InvalidAmount;
        }
        if(string.IsNullOrEmpty(command.ExternalId) || string.IsNullOrEmpty(command.ProviderCode) 
            || string.IsNullOrEmpty(command.OperationId) || string.IsNullOrEmpty(command.Action)) 
        {
            return ErrorCode.InvalidData;
        }
        return ErrorCode.Success;
    }
}
