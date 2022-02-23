using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tgs.CashService;

namespace Tgs.CashServiceWorker;

internal class WalletExchanger
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Guid, WalletReception> _exchangeQueuePool = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public WalletExchanger(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public Task<ExchangeResult> RequestAsync(ExchangeRequest request)
    {
        if (_cancellationTokenSource.IsCancellationRequested)
            throw new TaskCanceledException();

        var queue = _exchangeQueuePool.GetOrAdd(
            request.WalletId,
            _ => new WalletReception(
                request.WalletId,
                _serviceProvider,
                _serviceProvider.GetRequiredService<ILogger<WalletReception>>()));

        if (!queue.IsStartReceive)
            queue.StartReceive(_cancellationTokenSource.Token);

        var exchangeTask = new ExchangeTask(request, new TaskCompletionSource<ExchangeResult>());
        queue.Enqueue(exchangeTask);

        return exchangeTask.Promise.Task;
    }

    public void StopReceive()
    {
        if (!_cancellationTokenSource.IsCancellationRequested)
            _cancellationTokenSource.Cancel();

        Task.WaitAll(_exchangeQueuePool
            .Values
            .Select(reception => Task.Run(() => reception.StopReceive()))
            .ToArray());
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
                {
                    reception.StopReceive();

                    reception.Dispose();
                }
            }))
            .ToArray());
    }
}
