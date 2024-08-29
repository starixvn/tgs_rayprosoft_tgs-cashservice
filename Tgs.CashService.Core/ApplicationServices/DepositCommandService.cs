using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

[Obsolete("已淘汰")]
internal class DepositCommandService(ITgsRepository tgsRepository)
    : IGetService<DepositCommand, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public ValueTask<ExchangeResult> GetAsync(DepositCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.DepositOld,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
        );

        return _tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
