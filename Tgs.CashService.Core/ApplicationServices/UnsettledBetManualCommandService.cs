using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class UnsettledBetManualCommandService(ITgsRepository tgsRepository) : IGetService<UnsettledBetManualCommand, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<ExchangeResult> GetAsync(UnsettledBetManualCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.UnsettledBetManual,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
        );

        return await _tgsRepository.ExchangeWalletAsync(record, cancellationToken)
            .ConfigureAwait(false);
    }
}
