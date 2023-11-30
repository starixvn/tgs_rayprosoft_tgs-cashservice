using System.Diagnostics;
using System.Threading.Channels;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tgs.Cash.Protos;
using Tgs.CashService;
using Tgs.CashServiceWorker.ExchangeCommands;
using Valhalla.MessageQueue;

namespace Tgs.CashServiceWorker;

internal class WalletReception(
    Guid walletId,
    IServiceProvider serviceProvider,
    IMessageSender messageSender,
    ILogger<WalletReception> logger,
    CancellationToken cancellationToken)
    : IWalletExchangeContext
    , IDisposable
{
    private static readonly ActivitySource _ActivitySource = new("Tgs.CashServiceWorker.WalletReception");

    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly ILogger<WalletReception> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly Channel<IExchangeCommand> _exchangeTasks = Channel.CreateUnbounded<IExchangeCommand>(
        new UnboundedChannelOptions
        {
            SingleReader = true
        });

    private readonly CancellationTokenSource _cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    private Task? _backgroundTask;
    private bool _disposedValue;
    private decimal _balance;
    private decimal _lockedBalance;
    private Guid? _brandId;
    private string? _userId;

    public bool IsStartReceive { get; private set; } = false;

    public DateTime Expired { get; set; } = DateTime.Now.AddMinutes(10D).AddSeconds(20D);

    public Guid WalletId { get; } = walletId;

    Guid IWalletExchangeContext.BrandId => _brandId!.Value;

    string IWalletExchangeContext.UserId => _userId!;

    decimal IWalletExchangeContext.Balance
    {
        get => _balance;
        set => _balance = value;
    }

    decimal IWalletExchangeContext.LockedBalance
    {
        get => _lockedBalance;
        set => _lockedBalance = value;
    }

    public ValueTask EnqueueAsync(IExchangeCommand command, CancellationToken cancellationToken = default)
        => _exchangeTasks.Writer.WriteAsync(command, cancellationToken);

    public async ValueTask StartReceiveAsync(CancellationToken cancellationToken = default)
    {
        if (IsStartReceive)
            return;

        using var activity = _ActivitySource.StartActivity("WalletReception start receive");

        var isSnapshotWallet = false; ;
        var scope = _serviceProvider.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            var repository = scope.ServiceProvider.GetRequiredService<IWalletRepository>();

            var wallet = await repository
                .GetWalletAsync(WalletId, cancellationToken)
                .ConfigureAwait(false)
                ?? throw new InvalidOperationException("Can't found wallet.");

            var newestRecord = await repository.GetNewestRecordAsync(
                WalletId,
                wallet.SnapshotId,
                cancellationToken).ConfigureAwait(false);

            if (newestRecord is not null)
                wallet = wallet with
                {
                    Balance = newestRecord.Balance,
                    LockedBalance = newestRecord.LockedBalance,
                    SnapshotId = newestRecord.Id
                };

            _brandId = wallet.BrandId;
            _userId = wallet.UserId;
            _balance = wallet.Balance;
            _lockedBalance = wallet.LockedBalance;

            if (wallet.SnapshotId is null)
                isSnapshotWallet = true;
        }

        IsStartReceive = true;
        _ = _cancellationSource.Token.Register(() => _exchangeTasks.Writer.Complete());
        _backgroundTask = ProcessExchangeLoopAsync(cancellationToken);
        _ = _cancellationSource.Token.Register(_backgroundTask.Wait);

        if (isSnapshotWallet)
        {
            var command = new SnapshotCommand();
            await EnqueueAsync(
                command,
                cancellationToken).ConfigureAwait(false);

            _ = await command.GetExchangeResultAsync().ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    async ValueTask IWalletExchangeContext.NotifyCommitAsync(CancellationToken cancellationToken)
    {
        var commitRequest = new CommitExchangeRequest
        {
            WalletId = WalletId
        };

        await NewThreadAwaitable.Default;

        _ = messageSender.PublishAsync(
            "tgs.cash.commit.batch",
            commitRequest.ToByteArray(),
            cancellationToken)
            .AsTask();
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
            if (reader.TryRead(out var command))
                try
                {
                    Expired = DateTime.Now.AddMinutes(10D).AddSeconds(20D);

                    await HandleExchangeAsync(
                        command,
                        _cancellationSource!.Token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Process command occur errors.");
                }
        }
    }

    private async Task HandleExchangeAsync(IExchangeCommand command, CancellationToken cancellationToken)
    {
        using var activity = _ActivitySource.StartActivity(
            "WalletReception handle exchange",
            ActivityKind.Internal,
            parentContext: command.Context ?? default);

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                command.Cancel();
                return;
            }

            var scope = _serviceProvider.CreateAsyncScope();
            await using (scope.ConfigureAwait(false))
            {
                await command.ExecuteAsync(
                    this,
                    scope.ServiceProvider,
                    cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                new EventId(0),
                ex,
                "Process wallet({walletId}) exchange occur error.",
                WalletId);

            command.Fail(ex);
        }
    }
}
