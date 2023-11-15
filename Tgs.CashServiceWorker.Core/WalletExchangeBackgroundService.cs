using Microsoft.Extensions.Hosting;

namespace Tgs.CashServiceWorker;

internal class WalletExchangeBackgroundService : BackgroundService
{
    private readonly WalletExchanger _walletExchanger;

    public WalletExchangeBackgroundService(WalletExchanger walletExchanger)
    {
        _walletExchanger = walletExchanger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await NewThreadAwaitable.Default;

        while (!stoppingToken.IsCancellationRequested)
        {
            _walletExchanger.ScanExpiredQueues();

            await Task.Delay(
                10000,
                stoppingToken).ConfigureAwait(false);
        }
    }
}
