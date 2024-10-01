using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransferRollbackCommandService(ITgsRepository tgsRepository) : IGetService<TransferRollbackCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(TransferRollbackCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
           command.WalletId,
           TransactionType.TransferRollback,
           command.Amount,
           -command.Amount,
           command.Description ?? string.Empty,
           command.ExternalId.ToString(),
           command.Executor
       );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
