using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransactionLogQueryService(ITgsRepository tgsRepository)
        : IQueryService<TransactionLogQuery, TransactionLog>
    , IGetService<TransactionLogQuery, int>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public IAsyncEnumerable<TransactionLog> QueryAsync(TransactionLogQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.GetPlayerCodeByWalletIdAsync(query.WalletId, cancellationToken)
            .AsTask()
            .ToAsyncEnumerable()
            .Where(playerCode => !string.IsNullOrEmpty(playerCode))
            .SelectMany(playerCode => _tgsRepository.QueryTransactionLogsAsync(
                query.WalletId,
                playerCode!,
                query.QueryRange,
                query.QueryTypes,
                query.Paging,
                cancellationToken));

    public async ValueTask<int> GetAsync(TransactionLogQuery query, CancellationToken cancellationToken = default)
    {
        var playerCode = await _tgsRepository.GetPlayerCodeByWalletIdAsync(
            query.WalletId,
            cancellationToken).ConfigureAwait(false);
        return await _tgsRepository.CountTransactionLogsQueryAsync(
            query.WalletId,
            playerCode!,
            query.QueryRange,
            query.QueryTypes,
            cancellationToken).ConfigureAwait(false);
    }
}
