using Tgs.CashService;
using Tgs.ServiceUtility;

namespace Tgs.CashServiceWorker.ApplicationServices;

internal class ExchangeWalletCommandService : IGetService<ExchangeWalletCommand, ExchangeResult>
{
    private readonly WalletExchanger _walletExchanger;

    public ExchangeWalletCommandService(WalletExchanger walletExchanger)
    {
        _walletExchanger = walletExchanger ?? throw new ArgumentNullException(nameof(walletExchanger));
    }

    public async ValueTask<ExchangeResult> GetAsync(ExchangeWalletCommand command, CancellationToken cancellationToken = default)
    {
        var request = new ExchangeRequest(
            command.WalletId,
            command.TransactionType,
            command.Amount,
            command.LockAmount,
            command.Description,
            command.ExternalId,
            command.CreatedByUserId);

        return await _walletExchanger.RequestAsync(request).ConfigureAwait(false);
    }
}
