using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class ReceiveRollbackCommandService(ITgsRepository tgsRepository) : IGetService<ReceiveRollbackCommand, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public ValueTask<ExchangeResult> GetAsync(ReceiveRollbackCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.ReceiveRollback,
            0,
            -command.Amount,
            command.Description ?? string.Empty,
            command.ExternalId.ToString(),
            command.Executor
        );

        return _tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
