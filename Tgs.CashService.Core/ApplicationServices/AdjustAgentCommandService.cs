using System.Text.Json;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustAgentCommandService(ITgsRepository tgsRepository)
    : IGetService<AdjustAgentV2Command, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(AdjustAgentV2Command command, CancellationToken cancellationToken = default)
    {
        var description = new
        {
            TransactionType = (int)TransactionType.AdjustAgent,
            Remark = command.Description
        };

        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.AdjustAgent,
            command.Amount,
            0,
            JsonSerializer.Serialize(description),
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
            );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
