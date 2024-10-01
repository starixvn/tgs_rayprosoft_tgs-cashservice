using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class RebateCommandService(ITgsRepository tgsRepository) : IGetService<RebateCommand, ExchangeResult>
{
    public async ValueTask<ExchangeResult> GetAsync(RebateCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.Rebate,
            command.Amount.Truncate(2),
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return await tgsRepository.ExchangeWalletAsync(record, cancellationToken)
            .ConfigureAwait(false);
    }
}
