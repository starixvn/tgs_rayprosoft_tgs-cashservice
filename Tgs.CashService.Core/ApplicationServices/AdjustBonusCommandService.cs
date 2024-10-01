using System.Text.Json;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustBonusCommandService(ITgsRepository tgsRepository)
    : IGetService<AdjustBonusV2Command, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(AdjustBonusV2Command command, CancellationToken cancellationToken = default)
    {
        var description = new
        {
            TransactionType = (int)TransactionType.AdjustBonus,
            Remark = command.Description
        };

        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.AdjustBonus,
            command.Amount,
            0,
            JsonSerializer.Serialize(description),
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
        );
        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
