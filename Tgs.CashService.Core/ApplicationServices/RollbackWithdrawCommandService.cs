using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class RollbackWithdrawCommandService(ITgsRepository tgsRepository) : IGetService<RollbackWithdrawCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(RollbackWithdrawCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.WithdrawRollback,
            command.Amount,
            -command.Amount,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
