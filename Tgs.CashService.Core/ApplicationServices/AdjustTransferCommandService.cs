using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustTransferCommandService(ITgsRepository tgsRepository)
    : IGetService<AdjustTransferCommand, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

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

        return _tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
