using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class ReceiveCommitCommandService(ITgsRepository tgsRepository) : IGetService<ReceiveCommitCommand, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public ValueTask<ExchangeResult> GetAsync(ReceiveCommitCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.ReceiveCommit,
            command.Amount,
            -command.Amount,
            command.Description ?? string.Empty,
            command.ExternalId.ToString(),
            command.Executor
        );

        return _tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
