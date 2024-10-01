using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class ReceiveLockCommandService(ITgsRepository tgsRepository)
    : IGetService<ReceiveLockCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(ReceiveLockCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.Receive,
            0,
            command.Amount,
            command.Description ?? string.Empty,
            command.ExternalId.ToString(),
            command.Executor
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
