using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class DepositedCommandService(ITgsRepository tgsRepository)
    : IGetService<DepositedCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(DepositedCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.Deposit,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
