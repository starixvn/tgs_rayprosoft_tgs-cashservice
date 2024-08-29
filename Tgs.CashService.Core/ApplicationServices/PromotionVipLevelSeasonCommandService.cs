using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class PromotionVipLevelSeasonCommandService(ITgsRepository tgsRepository)
    : IGetService<PromotionVipLevelSeasonCommand, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<ExchangeResult> GetAsync(PromotionVipLevelSeasonCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.PromotionVipLevelSeason,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return await _tgsRepository.ExchangeWalletAsync(record, cancellationToken)
            .ConfigureAwait(false);
    }
}
