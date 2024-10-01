using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustPlayerNetProfitQueryService(ITgsRepository tgsRepository) : IQueryService<AdjustPlayerNetProfitQuery, PlayerAdjustRecord>
{
    public IAsyncEnumerable<PlayerAdjustRecord> QueryAsync(AdjustPlayerNetProfitQuery query, CancellationToken cancellationToken = default)
        => tgsRepository.QueryPlayerAdjustNetProfitAsync(query.TransactionIds, cancellationToken);
}
