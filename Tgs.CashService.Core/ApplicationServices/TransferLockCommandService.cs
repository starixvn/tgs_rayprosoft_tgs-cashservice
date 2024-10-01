using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransferLockCommandService(ITgsRepository tgsRepository) : IGetService<TransferLockCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(TransferLockCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.Transfer,
            -command.Amount,
            command.Amount,
            command.Description ?? string.Empty,
            command.ExternalId.ToString(),
            command.Executor
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
