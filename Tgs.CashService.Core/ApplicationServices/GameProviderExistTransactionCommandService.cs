using System.Text.Json;

using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderExistTransactionCommandService(ITgsRepository tgsRepository,
    ILogger<GameProviderExistTransactionCommand> logger)
    : IGetService<GameProviderExistTransactionCommand, bool>
{
    public async ValueTask<bool> GetAsync(GameProviderExistTransactionCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(command.ExternalId) || string.IsNullOrEmpty(command.ProviderCode)
        || string.IsNullOrEmpty(command.Action))
        {
            logger.LogError($"Exist Transaction - {ErrorMessageMapper.Messages[ErrorCode.InvalidData]} {JsonSerializer.Serialize(command)}");
            throw new ArgumentNullException(ErrorMessageMapper.Messages[ErrorCode.InvalidData]);
        }

        var transactionId = await tgsRepository
            .GetTransactionIdByProviderInfoAsync(command.PlayerCode, new ProviderInfo(command.ProviderCode, command.ExternalId, command.Action),
            cancellationToken)
            .ConfigureAwait(false);
        return !string.IsNullOrEmpty(transactionId);
    }
}
