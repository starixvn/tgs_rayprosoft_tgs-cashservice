using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class WithdrawLockCommandService(ITgsRepository tgsRepository) : IGetService<WithdrawLockCommand, ExchangeResult>
{
    public async ValueTask<ExchangeResult> GetAsync(WithdrawLockCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.WithdrawLock,
            -command.Amount,
            command.Amount,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return await tgsRepository.ExchangeWalletAsync(record, cancellationToken)
            .ConfigureAwait(false);
    }
}
