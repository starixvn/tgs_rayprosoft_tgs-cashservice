using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustTransferCommandService(ITgsRepository tgsRepository)
    : IGetService<AdjustTransferCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(AdjustTransferCommand command,
        CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.AdjustTransfer,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
