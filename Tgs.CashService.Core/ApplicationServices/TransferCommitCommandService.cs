using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransferCommitCommandService(ITgsRepository tgsRepository) : IGetService<TransferCommitCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(TransferCommitCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.TransferCommit,
            0,
            -command.Amount,
            command.Description ?? string.Empty,
            command.ExternalId.ToString(),
            command.Executor
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
