using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderFundRefundCommandService(ITgsRepository tgsRepository)
    : IGetService<GameProviderFundRefundCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(GameProviderFundRefundCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.GameProviderFundRefund,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
