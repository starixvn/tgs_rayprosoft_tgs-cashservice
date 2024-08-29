using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class WithdrawLockCommandService(ITgsRepository tgsRepository) : IGetService<WithdrawLockCommand, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

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

        return await _tgsRepository.ExchangeWalletAsync(record, cancellationToken)
            .ConfigureAwait(false);
    }
}
