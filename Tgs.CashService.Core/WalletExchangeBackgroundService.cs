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

        return Task.CompletedTask;
    }
}
