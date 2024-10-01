using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderFundInCommandService(ITgsRepository tgsRepository)
    : IGetService<GameProviderFundInCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(GameProviderFundInCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.GameProviderFundIn,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
