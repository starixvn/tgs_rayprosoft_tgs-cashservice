using Tgs.CashService;

namespace Tgs.CashServiceWorker.ExchangeCommands;

internal class ReadCommand : IExchangeCommand
{
    private readonly TaskCompletionSource<WalletView> _promise = new();

    public void Cancel()
            => _promise.TrySetCanceled();

    public void Fail(Exception ex)
        => _promise.TrySetException(ex);

    public ValueTask ExecuteAsync(
        IWalletExchangeContext context,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        _promise.SetResult(new WalletView(
            context.Balance,
            context.LockedBalance));

        return ValueTask.CompletedTask;
    }

    public Task<WalletView> GetBalanceResultAsync()
        => _promise.Task;
}
