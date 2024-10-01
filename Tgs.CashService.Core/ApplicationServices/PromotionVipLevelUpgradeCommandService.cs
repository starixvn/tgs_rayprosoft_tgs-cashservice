using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class PromotionVipLevelupgradeCommandService(ITgsRepository tgsRepository)
    : IGetService<PromotionVipLevelUpgradeCommand, ExchangeResult>
{
    public async ValueTask<ExchangeResult> GetAsync(PromotionVipLevelUpgradeCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.PromotionVipLevelUpgrade,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return await tgsRepository.ExchangeWalletAsync(record, cancellationToken)
            .ConfigureAwait(false);
    }
}
