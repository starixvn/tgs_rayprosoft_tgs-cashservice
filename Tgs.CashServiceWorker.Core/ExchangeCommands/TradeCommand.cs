using Microsoft.Extensions.DependencyInjection;
using Tgs.CashService;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashServiceWorker.ExchangeCommands;

internal class TradeCommand(ExchangeRequest request) : IExchangeCommand
{
    private readonly TaskCompletionSource<ExchangeResult> _promise = new();

    public void Cancel()
            => _promise.TrySetCanceled();

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

        context.LockedBalance += request.LockAmount;
        if (context.LockedBalance < 0M)
        {
            context.LockedBalance -= request.LockAmount;

            throw new WalletExchangeFailException();
        }
        context.Balance += request.Amount;

        var record = new TransactionRecord(
            transactionId,
            context.WalletId,
            context.BrandId,
            context.UserId,
            request.Amount,
            request.LockAmount,
            context.Balance,
            context.LockedBalance,
            request.TransactionType,
            request.Description,
            currentTime,
            request.ExternalId,
            request.CreatedByUserId);

        await repository.WriteTransactionRecordAsync(
            record,
            cancellationToken).ConfigureAwait(false);

        await context.NotifyCommitAsync(cancellationToken).ConfigureAwait(false);

        _promise.SetResult(new ExchangeResult(
            transactionId,
            context.Balance,
            context.LockedBalance));
    }

    public void Fail(Exception ex)
        => _promise.TrySetException(ex);

    internal Task<ExchangeResult> GetExchangeResultAsync()
        => _promise.Task;
}
