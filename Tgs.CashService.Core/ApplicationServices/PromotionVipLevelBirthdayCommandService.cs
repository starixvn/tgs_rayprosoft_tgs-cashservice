using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class PromotionVipLevelBirthdayCommandService(ITgsRepository tgsRepository)
    : IGetService<PromotionVipLevelBirthdayCommand, ExchangeResult>
{
    public async ValueTask<ExchangeResult> GetAsync(PromotionVipLevelBirthdayCommand command,
        CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.PromotionVipLevelBirthday,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return await tgsRepository.ExchangeWalletAsync(record, cancellationToken)
            .ConfigureAwait(false);
    }
}
