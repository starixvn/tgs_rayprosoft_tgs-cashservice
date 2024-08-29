using System.Text.Json;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustSystemCommandService(ITgsRepository tgsRepository)
    : IGetService<AdjustSystemV2Command, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public ValueTask<ExchangeResult> GetAsync(AdjustSystemV2Command command, CancellationToken cancellationToken = default)
    {
        var description = new
        {
            TransactionType = (int)TransactionType.AdjustSystem,
            Remark = command.Description
        };
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.AdjustSystem,
            command.Amount,
            command.LockAmount,
            JsonSerializer.Serialize(description),
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
        );

        return _tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
