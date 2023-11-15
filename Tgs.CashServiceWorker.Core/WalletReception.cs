using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tgs.CashServiceWorker;

internal class WalletReception(
    Guid walletId,
    IServiceProvider serviceProvider,
    ILogger<WalletReception> logger,
    CancellationToken cancellationToken) : IDisposable
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly ILogger<WalletReception> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly Channel<ExchangeTask> _exchangeTasks = Channel.CreateUnbounded<ExchangeTask>(
        new UnboundedChannelOptions
        {
            SingleReader = true
        });

    private readonly CancellationTokenSource _cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    private Task? _backgroundTask;
    private bool _disposedValue;

    public bool IsStartReceive { get; private set; } = false;

    public DateTime Expired { get; set; } = DateTime.Now.AddMinutes(1).AddSeconds(20);

    public Guid WalletId { get; } = walletId;

    public ValueTask EnqueueAsync(ExchangeTask exchangeTask, CancellationToken cancellationToken = default)
        => _exchangeTasks.Writer.WriteAsync(exchangeTask, cancellationToken);

    public ValueTask StartReceiveAsync(CancellationToken cancellationToken = default)
    {
        if (IsStartReceive)
            return ValueTask.CompletedTask;

        IsStartReceive = true;
        _ = _cancellationSource.Token.Register(() => _exchangeTasks.Writer.Complete());
        _backgroundTask = ProcessExchangeLoopAsync(cancellationToken);
        _ = _cancellationSource.Token.Register(_backgroundTask.Wait);

        return ValueTask.CompletedTask;
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

                _cancellationSource?.Dispose();
            }

            _disposedValue = true;
        }
    }

    private async Task ProcessExchangeLoopAsync(CancellationToken cancellationToken)
    {
        await NewThreadAwaitable.Default;

        var reader = _exchangeTasks.Reader;
        while (await reader
            .WaitToReadAsync(cancellationToken)
            .ConfigureAwait(false))
        {
            if (reader.TryRead(out var exchangeTask))
                try
                {
                    Expired = DateTime.Now.AddMinutes(1).AddSeconds(20);

                    await HandleExchangeAsync(
                        exchangeTask,
                        _cancellationSource!.Token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Process command occur errors.");
                }
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

            var scope = _serviceProvider.CreateAsyncScope();
            await using (scope.ConfigureAwait(false))
            {
                var repository = scope.ServiceProvider.GetRequiredService<IWalletRepository>();

                var wallet = await repository
                    .GetWalletAsync(exchangeTask.Request.WalletId, cancellationToken)
                    .ConfigureAwait(false)
                    ?? throw new InvalidOperationException("Can't found wallet.");

                var result = await repository.ExchangeAsync(
                    wallet.BrandId,
                    exchangeTask.Request.WalletId,
                    wallet.PlayerCode,
                    exchangeTask.Request.TransactionType,
                    exchangeTask.Request.Amount,
                    exchangeTask.Request.LockAmount,
                    exchangeTask.Request.Description,
                    exchangeTask.Request.ExternalId,
                    exchangeTask.Request.CreatedByUserId,
                    cancellationToken).ConfigureAwait(false);

                exchangeTask.Promise.SetResult(result);
            }
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
