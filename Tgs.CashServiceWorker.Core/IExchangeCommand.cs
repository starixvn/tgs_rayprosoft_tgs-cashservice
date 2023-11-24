namespace Tgs.CashServiceWorker;

internal interface IExchangeCommand
{
    void Cancel();

    void Fail(Exception ex);

    ValueTask ExecuteAsync(
        IWalletExchangeContext context,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default);
}
