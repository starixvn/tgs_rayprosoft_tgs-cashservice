using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class DividendAllAgentRebateCommandService(ITgsRepository tgsRepository)
    : IGetService<DividendAllAgentRebateCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(
        DividendAllAgentRebateCommand command,
        CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.DividendAllAgentRebate,
            command.Amount.Truncate(2),
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
