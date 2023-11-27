using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustPlayerNetProfitQueryService(ITgsRepository tgsRepository) : IQueryService<AdjustPlayerNetProfitQuery, PlayerAdjustRecord>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public IAsyncEnumerable<PlayerAdjustRecord> QueryAsync(AdjustPlayerNetProfitQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.QueryPlayerAdjustNetProfitAsync(query.TransactionIds, cancellationToken);
}
