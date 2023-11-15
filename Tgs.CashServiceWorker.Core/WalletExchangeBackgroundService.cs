using Microsoft.Extensions.Hosting;

namespace Tgs.CashServiceWorker;

internal class WalletExchangeBackgroundService(WalletExchanger walletExchanger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await NewThreadAwaitable.Default;

        while (!stoppingToken.IsCancellationRequested)
        {
            walletExchanger.ScanExpiredQueues();

            await Task.Delay(
                10000,
                stoppingToken).ConfigureAwait(false);
        }
    }
}
