using Microsoft.Extensions.Hosting;

namespace Tgs.CashService.Core;

internal class WalletExchangeBackgroundService : BackgroundService
{
    private readonly WalletExchanger _walletExchanger;

    public WalletExchangeBackgroundService(WalletExchanger walletExchanger)
    {
        _walletExchanger = walletExchanger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = stoppingToken.Register(() => _walletExchanger.StopReceive());

        return Task.Run(async () =>
        {
            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                _walletExchanger.ScanExpiredQueues();

                await Task.Delay(10000, stoppingToken);
            }
        }, stoppingToken);
    }
}
