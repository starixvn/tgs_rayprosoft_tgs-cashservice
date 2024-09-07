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
            var errorCode = await IsValidData(command, cancellationToken).ConfigureAwait(false);
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
            command.PlayerCode
            );

            var result = await _tgsRepository.ExchangeWalletAsync(record, cancellationToken)
                .ConfigureAwait(false);
            return new BalanceResult(result.TransactionId, result.Balance, result.LockedBalance);
        }
        catch (Exception ex) {
            logger.LogError(ex.Message);
            var code = ExceptionMapper.GetErrorCodeByException(ex);
            return new BalanceResult(Guid.Empty, 0, 0, code);
        }
    }

    private async ValueTask<string> IsValidData(GameProviderAddBalanceCommand command, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrEmpty(command.ExternalId)) 
        {
            return ErrorCode.InvalidData;
        }
        var transaction = await _tgsRepository
                .GetTransactionLogByExternalIdAsync(command.ExternalId, cancellationToken)
                .ConfigureAwait(false);
        return transaction != null ? ErrorCode.DuplicateTransaction : ErrorCode.Success;
    }
}
