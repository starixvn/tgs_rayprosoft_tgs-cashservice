using System.Text.Json;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustSystemCommandService(ITgsRepository tgsRepository)
    : IGetService<AdjustSystemV2Command, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(AdjustSystemV2Command command, CancellationToken cancellationToken = default)
    {
        var description = new
        {
            TransactionType = (int)TransactionType.AdjustSystem,
            Remark = command.Description
        };
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.AdjustSystem,
            command.Amount,
            command.LockAmount,
            JsonSerializer.Serialize(description),
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
