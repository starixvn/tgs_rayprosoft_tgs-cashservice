using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderDepositCommandService(ITgsRepository tgsRepository)
    : IGetService<GameProviderDepositCommand, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public ValueTask<ExchangeResult> GetAsync(GameProviderDepositCommand command, CancellationToken cancellationToken = default)
    {
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.GameProviderDeposit,
            -command.Amount,
            0,
            command.Description ?? string.Empty,
            command.ExternalId ?? string.Empty
        );

        return _tgsRepository.ExchangeWalletAsync(record, cancellationToken);
;
    }
}
