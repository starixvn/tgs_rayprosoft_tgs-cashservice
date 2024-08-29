using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustResettledBetManualCommandService(ITgsRepository tgsRepository)
    : IGetService<AdjustResettledBetManualCommand, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public ValueTask<ExchangeResult> GetAsync(AdjustResettledBetManualCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.AdjustResettledBetManual,
            command.Amount,
            0,
            command.Description ?? String.Empty,
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
        );

        return _tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
