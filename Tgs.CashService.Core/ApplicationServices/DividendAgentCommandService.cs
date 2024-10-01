using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class DividendAgentCommandService(ITgsRepository tgsRepository)
    : IGetService<DividendAgentCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(DividendAgentCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.AgentDividend,
            command.Amount.Truncate(2),
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
