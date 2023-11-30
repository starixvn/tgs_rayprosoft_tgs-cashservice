using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Tgs.CashService;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashServiceWorker.ExchangeCommands;

internal class SnapshotCommand(ActivityContext? activityContext) : IExchangeCommand
{
    private readonly TaskCompletionSource<ExchangeResult> _promise = new();

    public ActivityContext? Context => activityContext;

    public void Cancel()
        => _promise.TrySetCanceled();

    public void Fail(Exception ex)
        => _promise.TrySetException(ex);

    public async ValueTask ExecuteAsync(
        IWalletExchangeContext context,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var repository = serviceProvider.GetRequiredService<IWalletRepository>();
        var snGenerator = serviceProvider.GetRequiredService<ISerialNumberGenerator>();

        var currentTime = DateTime.UtcNow;
        var transactionId = await snGenerator
            .GenerateAsync(cancellationToken)
            .ConfigureAwait(false);

        var record = new TransactionRecord(
            transactionId,
            context.WalletId,
            context.BrandId,
            context.UserId,
            0M,
            0M,
            context.Balance,
            context.LockedBalance,
            TransactionType.Snapshot,
            "Snapshot to upgrade",
            currentTime,
            null,
            null);

        await repository.WriteTransactionRecordAsync(
            record,
            cancellationToken).ConfigureAwait(false);

        await context.NotifyCommitAsync(cancellationToken).ConfigureAwait(false);

        _promise.SetResult(new ExchangeResult(
            transactionId,
            context.Balance,
            context.LockedBalance));
    }

    internal Task<ExchangeResult> GetExchangeResultAsync()
        => _promise.Task;
}
