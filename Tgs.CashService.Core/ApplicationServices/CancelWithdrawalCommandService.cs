using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

[Obsolete("已淘汰")]
internal class CancelWithdrawalCommandService( ITgsRepository tgsRepository)
    : IGetService<CancelWithdrawalCommand, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public ValueTask<ExchangeResult> GetAsync(CancelWithdrawalCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.CancelWithdrawal,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
        );

        return _tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
