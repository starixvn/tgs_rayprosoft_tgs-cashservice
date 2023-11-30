using System.Diagnostics;

namespace Tgs.CashServiceWorker;

internal interface IExchangeCommand
{
    ActivityContext? Context { get; }

    void Cancel();

    void Fail(Exception ex);

    ValueTask ExecuteAsync(
        IWalletExchangeContext context,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default);
}
