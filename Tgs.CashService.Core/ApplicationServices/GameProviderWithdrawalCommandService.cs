using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderWithdrawalCommandService(ITgsRepository tgsRepository)
    : IGetService<GameProviderWithdrawalCommand, ExchangeResult>
{
    public ValueTask<ExchangeResult> GetAsync(GameProviderWithdrawalCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.GameProviderWithdrawal,
            command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
