using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Tgs.CashService;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashServiceWorker.ExchangeCommands;

internal class DeductBalanceCommand(
    ExchangeRequest request,
    ActivityContext? activityContext) : TradeCommand(request, activityContext)
{
    public override async ValueTask ExecuteAsync(
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

        if (context.Balance < request.Amount)
        {
            throw new WalletExchangeFailException("Not enough balance");
        }
        context.Balance -= request.Amount;

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

        try
        {
            await repository.WriteTransactionRecordAsync(
                record,
                cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            context.Balance += request.Amount;

            throw;
        }

        await context.NotifyCommitAsync(cancellationToken).ConfigureAwait(false);

        _promise.SetResult(new ExchangeResult(
            transactionId,
            context.Balance,
            context.LockedBalance));
    }
}
