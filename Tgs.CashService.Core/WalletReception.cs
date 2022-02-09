using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tgs.CashService.Core;

internal class WalletReception : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WalletReception> _logger;
    private readonly BlockingCollection<ExchangeTask> _exchangeTasks = new();
    private Task? _backgroundTask;
    private CancellationTokenSource? _cancellationSource;
    private bool _disposedValue;

    public bool IsStartReceive { get; private set; } = false;

    public DateTime Expired { get; set; } = DateTime.Now.AddMinutes(1).AddSeconds(20);

    public Guid WalletId { get; }

    public WalletReception(Guid walletId, IServiceProvider serviceProvider, ILogger<WalletReception> logger)
    {
        WalletId = walletId;
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Enqueue(ExchangeTask exchangeTask)
        => _exchangeTasks.Add(exchangeTask);

    public void StartReceive(CancellationToken cancellationToken)
    {
        if (IsStartReceive)
            return;

        IsStartReceive = true;

        _cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _ = _cancellationSource.Token.Register(() => _exchangeTasks.CompleteAdding());

        _backgroundTask = Task.Run(async () =>
        {
            foreach (var exchangeTask in _exchangeTasks.GetConsumingEnumerable(_cancellationSource.Token))
            {
                Expired = DateTime.Now.AddMinutes(1).AddSeconds(20);

                await HandleExchangeAsync(
                    exchangeTask,
                    _cancellationSource.Token).ConfigureAwait(false);
            }
        }, cancellationToken);
    }

    public void StopReceive()
    {
        if (_cancellationSource?.IsCancellationRequested == false)
            _cancellationSource?.Cancel();

        try
        {
            if (_exchangeTasks.TryGetNonEnumeratedCount(out var count)
                && count > 0
                && _backgroundTask?.IsCompleted == false)
                _backgroundTask?.Wait();
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
        {
        }
    }

    public void Dispose()
    {
        // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_cancellationSource?.IsCancellationRequested == false)
                    _cancellationSource.Cancel();

                _exchangeTasks.CompleteAdding();
            }

            // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
            // TODO: 將大型欄位設為 Null
            _disposedValue = true;
        }
    }

    private async Task HandleExchangeAsync(ExchangeTask exchangeTask, CancellationToken cancellationToken)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _ = exchangeTask.Promise.TrySetCanceled(cancellationToken);
                return;
            }

            await using var scope = _serviceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<IWalletRepository>();

            var result = await repository.ExchangeAsync(
                exchangeTask.Request.BrandId,
                exchangeTask.Request.WalletId,
                exchangeTask.Request.PlayerCode,
                exchangeTask.Request.TransactionType,
                exchangeTask.Request.Amount,
                exchangeTask.Request.Description,
                exchangeTask.Request.ExternalId,
                exchangeTask.Request.CreatedByUserId).ConfigureAwait(false);

            exchangeTask.Promise.SetResult(result);
        }
        catch (Exception ex)
        {
            _logger.Log(
                LogLevel.Error,
                new EventId(0),
                exchangeTask.Request,
                ex,
                (state, _) => $"Process wallet({state.WalletId}) exchange occur error.");

            exchangeTask.Promise.SetException(ex);
        }
    }
}
