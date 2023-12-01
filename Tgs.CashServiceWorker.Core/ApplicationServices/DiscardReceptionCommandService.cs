using Tgs.ServiceUtility;

namespace Tgs.CashServiceWorker.ApplicationServices;

internal class DiscardReceptionCommandService(
    WalletExchanger walletExchanger)
    : ICommandService<DiscardReceptionCommand>
{
    public async ValueTask ExecuteAsync(DiscardReceptionCommand command, CancellationToken cancellationToken = default)
    {
        await walletExchanger.DiscardReceptionAsync(command.WalletId).ConfigureAwait(false);
    }
}
