using Microsoft.Extensions.Hosting;
using Valhalla.Awaitables;

namespace Tgs.CashServiceWorker;

internal class WalletExchangeBackgroundService(WalletExchanger walletExchanger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await TaskHelper.NewThread();

        while (!stoppingToken.IsCancellationRequested)
        {
            walletExchanger.ScanExpiredQueues();

            await Task.Delay(
                10000,
                stoppingToken).ConfigureAwait(false);
        }
    }
}
