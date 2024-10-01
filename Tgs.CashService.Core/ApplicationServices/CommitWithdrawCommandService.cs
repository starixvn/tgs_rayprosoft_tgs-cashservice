using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class CommitWithdrawCommandService(ITgsRepository tgsRepository)
    : IGetService<CommitWithdrawCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(CommitWithdrawCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.WithdrawCommit,
            0,
            -command.Amount,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
