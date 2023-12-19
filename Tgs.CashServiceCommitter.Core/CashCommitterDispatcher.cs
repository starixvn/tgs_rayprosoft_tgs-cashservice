using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tgs.CashServiceCommitter.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.Awaitables;
using Valhalla.Messages;

namespace Tgs.CashServiceCommitter;

internal class CashCommitterDispatcher(
    IEventBus eventBus,
    IServiceProvider serviceProvider,
    ILogger<CashCommitterDispatcher> logger)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var walletChangedTask = Observable.Using(
            () => Disposable.Empty,
            res =>
            {
                var wallets = new ConcurrentDictionary<Guid, Guid>();

                return eventBus.GetEventObserable<WalletChangedEvent>(CommitterConsts.WalletChangedEventName)!
                    .GroupByUntil(
                        evt => evt.WalletId,
                        g =>
                        {
                            var waitSeconds = 300;
                            var trigger = new TaskCompletionSource();

                            return Observable.Using(
                                () => g.Subscribe(_ => Interlocked.Exchange(ref waitSeconds, 300)),
                                d => Observable.Using(
                                    () => new Timer(
                                        _ =>
                                        {
                                            _ = Interlocked.Decrement(ref waitSeconds);
                                            if (waitSeconds <= 0)
                                                trigger.SetResult();
                                        },
                                        null,
                                        0,
                                        1000),
                                    tm => Observable.FromAsync(() => trigger.Task)));
                        })
                    .Select(g => g
                        .Select(evt => (Event: evt, Nonce: Guid.NewGuid()))
                        .Where(tup => wallets.GetOrAdd(tup.Event.WalletId, key => tup.Nonce) == tup.Nonce)
                        .Select(tup => tup.Event)
                        .Select(evt => Observable.FromAsync(async () =>
                        {
                            _ = wallets.TryRemove(evt.WalletId, out _);

                            await TaskHelper.NewThread();

                            try
                            {
                                var scope = serviceProvider.CreateAsyncScope();
                                await using var r = scope.ConfigureAwait(false);

                                var bookkeepService = scope.ServiceProvider.GetRequiredService<ICommandService<BookkeepingWalletTransactionLogsCommand>>();
                                var command = new BookkeepingWalletTransactionLogsCommand(evt.WalletId);

                                await bookkeepService.ExecuteAsync(
                                    command,
                                    stoppingToken).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Process {walletId} event occur error!", evt.WalletId);
                            }
                        }))
                        .Concat())
                    .Merge();
            })
            .ToTask(stoppingToken);
        var updateWalletTask = Observable.Using(
            () => Disposable.Empty,
            res =>
            {
                var wallets = new ConcurrentDictionary<Guid, Guid>();

                return eventBus.GetEventObserable<WalletUpdateEvent>(CommitterConsts.WalletUpdateEventName)!
                    .GroupByUntil(
                        evt => evt.WalletId,
                        g =>
                        {
                            var waitSeconds = 300;
                            var trigger = new TaskCompletionSource();

                            return Observable.Using(
                                () => g.Subscribe(_ => Interlocked.Exchange(ref waitSeconds, 300)),
                                d => Observable.Using(
                                    () => new Timer(
                                        _ =>
                                        {
                                            _ = Interlocked.Decrement(ref waitSeconds);
                                            if (waitSeconds <= 0)
                                                trigger.SetResult();
                                        },
                                        null,
                                        0,
                                        1000),
                                    tm => Observable.FromAsync(() => trigger.Task)));
                        })
                    .Select(g => g
                        .Select(evt => (Event: evt, Nonce: Guid.NewGuid()))
                        .Where(tup => wallets.GetOrAdd(tup.Event.WalletId, key => tup.Nonce) == tup.Nonce)
                        .Select(tup => tup.Event)
                        .Select(evt => Observable.FromAsync(async () =>
                        {
                            _ = wallets.TryRemove(evt.WalletId, out _);

                            await NewThreadAwaitable.Default;

                            try
                            {
                                var scope = serviceProvider.CreateAsyncScope();
                                await using var r = scope.ConfigureAwait(false);

                                var updateService = scope.ServiceProvider.GetRequiredService<ICommandService<UpdateWalletCommand>>();
                                var command = new UpdateWalletCommand(evt.WalletId);

                                await updateService.ExecuteAsync(
                                    command,
                                    stoppingToken).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Process {walletId} event occur error!", evt.WalletId);
                            }
                        }))
                        .Concat())
                    .Merge();
            })
            .ToTask(stoppingToken);

        return Task.WhenAll(walletChangedTask, updateWalletTask);
    }
}
