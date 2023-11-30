using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tgs.CashService;
using Tgs.CashServiceWorker.ExchangeCommands;
using Valhalla.MessageQueue;

namespace Tgs.CashServiceWorker;

internal class WalletExchanger(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : IDisposable
{
    private static readonly ActivitySource _ActivitySource = new("Tgs.CashServiceWorker.WalletExchanger");
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly ConcurrentDictionary<Guid, WalletReception> _exchangeQueuePool = new();
    private readonly CancellationTokenSource _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(hostApplicationLifetime.ApplicationStopping);

    private bool _disposedValue;

    public async ValueTask<ExchangeResult> RequestAsync(ExchangeRequest request, CancellationToken cancellationToken = default)
    {
        using var activity = _ActivitySource.StartActivity(
            "WalletExchanger Request",
            ActivityKind.Internal,
            Activity.Current?.Context ?? default);

        if (_cancellationTokenSource.IsCancellationRequested)
            throw new TaskCanceledException();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var queue = await GetWalletReceptionAsync(
            request.WalletId).ConfigureAwait(false);

        var command = new TradeCommand(request);
        await using var tokenRegistration = cts.Token.Register(
            command.Cancel).ConfigureAwait(false);
        await queue.EnqueueAsync(
            command,
            cts.Token).ConfigureAwait(false);

        return await command.GetExchangeResultAsync().ConfigureAwait(false);
    }

    public async ValueTask<WalletView> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        using var activity = _ActivitySource.StartActivity(
            "WalletExchanger GetBalance",
            ActivityKind.Internal,
            Activity.Current?.Context ?? default);

        if (_cancellationTokenSource.IsCancellationRequested)
            throw new TaskCanceledException();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var queue = await GetWalletReceptionAsync(
            walletId).ConfigureAwait(false);

        var command = new ReadCommand();
        await using var tokenRegistration = cts.Token.Register(
            command.Cancel).ConfigureAwait(false);
        await queue.EnqueueAsync(
            command,
            cts.Token).ConfigureAwait(false);

        return await command.GetBalanceResultAsync().ConfigureAwait(false);
    }

    public void ScanExpiredQueues()
    {
        var currentTime = DateTime.Now;

        Task.WaitAll(_exchangeQueuePool
            .Values
            .Where(reception => reception.Expired < currentTime)
            .Select(reception => Task.Run(() =>
            {
                if (_exchangeQueuePool.TryRemove(reception.WalletId, out _))
                    reception.Dispose();
            }))
            .ToArray());
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _cancellationTokenSource.Cancel();

                var allValues = _exchangeQueuePool.Values;
                _exchangeQueuePool.Clear();

                Task.WaitAll(allValues
                    .Select(reception => Task.Run(() =>
                    {
                        if (_exchangeQueuePool.TryRemove(reception.WalletId, out _))
                            reception.Dispose();
                    }))
                    .ToArray());
            }

            _disposedValue = true;
        }
    }

    private async ValueTask<WalletReception> GetWalletReceptionAsync(Guid walletId)
    {
        using var activity = _ActivitySource.StartActivity(
            "WalletExchanger GetWalletReception",
            ActivityKind.Internal,
            Activity.Current?.Context ?? default);

        var queue = _exchangeQueuePool.GetOrAdd(
            walletId,
            _ => new WalletReception(
                walletId,
                _serviceProvider,
                _serviceProvider.GetRequiredService<IMessageSender>(),
                _serviceProvider.GetRequiredService<ILogger<WalletReception>>(),
                _cancellationTokenSource.Token));

        if (!queue.IsStartReceive)
            await queue.StartReceiveAsync(
                _cancellationTokenSource.Token).ConfigureAwait(false);

        return queue;
    }
}
