using Microsoft.Extensions.Logging;
using System.Text.Json;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderExistTransactionCommandService(ITgsRepository tgsRepository,
    ILogger<GameProviderExistTransactionCommand> logger)
    : IGetService<GameProviderExistTransactionCommand, bool>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<bool> GetAsync(GameProviderExistTransactionCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(command.ExternalId) || string.IsNullOrEmpty(command.ProviderCode)
        || string.IsNullOrEmpty(command.Action))
        {
            logger.LogError($"Exist Transaction - {ErrorMessages.Messages[ErrorCode.InvalidData]} {JsonSerializer.Serialize(command)}");
            throw new Exception(ErrorMessages.Messages[ErrorCode.InvalidData]);
        }

        var transaction = await _tgsRepository
            .GetTransactionLogByProviderInfoAsync(new ProviderInfo(command.ProviderCode, command.ExternalId, command.Action),
            cancellationToken)
            .ConfigureAwait(false);
        return transaction != null;
    }
}

