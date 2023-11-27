using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Tgs.CashService;
using Tgs.ServiceUtility;
using Valhalla.Messages;

namespace Tgs.CashServiceCommitter.ApplicationServices;

internal class BookkeepingWalletTransactionLogsCommandService(
    ITransactionLogRepository transcationLogRepository,
    ITgsRepository tgsRepository,
    IEventBus eventBus,
    ILogger<BookkeepingWalletTransactionLogsCommandService> logger)
    : ICommandService<BookkeepingWalletTransactionLogsCommand>
{
    public async ValueTask ExecuteAsync(
        BookkeepingWalletTransactionLogsCommand command,
        CancellationToken cancellationToken = default)
    {
        await foreach (var records in GetWaitingProcessBatchAsync(
            command.WalletId,
            cancellationToken).ConfigureAwait(false))
        {
            var processCount = records.Length;

            if (processCount == 0)
                break;

            try
            {
                await tgsRepository.InsertTransactionLogsAsync(
                    records,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "{transcationLogs}", records);

                await tgsRepository.MigrateTransactionLogsAsync(
                    records,
                    cancellationToken).ConfigureAwait(false);
            }

            await transcationLogRepository.RemoveBookkeepLogsAsync(
                command.WalletId,
                processCount,
                cancellationToken).ConfigureAwait(false);
        }

        _ = await eventBus.GetEventPublisher<WalletUpdateEvent>(CommitterConsts.WalletUpdateEventName)
            !.PublishAsync(
                new WalletUpdateEvent(command.WalletId),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async IAsyncEnumerable<TransactionRecord[]> GetWaitingProcessBatchAsync(
        Guid walletId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (true)
        {
            yield return await transcationLogRepository
                .GetWaitingBookkeepLogsAsync(walletId, cancellationToken)
                .ToArrayAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
