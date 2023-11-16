using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tgs.CashService;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashServiceWorker;

internal class WalletExchanger(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : IDisposable
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly ConcurrentDictionary<Guid, WalletReception> _exchangeQueuePool = new();
    private readonly CancellationTokenSource _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(hostApplicationLifetime.ApplicationStopping);

    private bool _disposedValue;

    public async ValueTask<ExchangeResult> RequestAsync(ExchangeRequest request, CancellationToken cancellationToken = default)
    {
        if (_cancellationTokenSource.IsCancellationRequested)
            throw new TaskCanceledException();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var queue = _exchangeQueuePool.GetOrAdd(
            request.WalletId,
            _ => new WalletReception(
                request.WalletId,
                _serviceProvider,
                _serviceProvider.GetRequiredService<ISerialNumberGenerator>(),
                _serviceProvider.GetRequiredService<ILogger<WalletReception>>(),
                _cancellationTokenSource.Token));

        if (!queue.IsStartReceive)
            await queue.StartReceiveAsync(
                _cancellationTokenSource.Token).ConfigureAwait(false);

        var exchangeTask = new ExchangeTask(request, new TaskCompletionSource<ExchangeResult>());

        await using var tokenRegistration = cts.Token.Register(
            () => exchangeTask.Promise.TrySetCanceled()).ConfigureAwait(false);
        await queue.EnqueueAsync(exchangeTask, cts.Token).ConfigureAwait(false);

        return await exchangeTask.Promise.Task.ConfigureAwait(false);
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
}
