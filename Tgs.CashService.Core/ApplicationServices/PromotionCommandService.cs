using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class PromotionCommandService(ITgsRepository tgsRepository) : IGetService<PromotionCommand, ExchangeResult>
{
    public async ValueTask<ExchangeResult> GetAsync(PromotionCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.Promotion,
            command.Amount.Truncate(2),
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return await tgsRepository.ExchangeWalletAsync(record, cancellationToken)
            .ConfigureAwait(false);
    }
}
