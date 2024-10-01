using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustPlayerSummaryQueryService(ITgsRepository tgsRepository)
        : IQueryService<AdjustPlayerSummaryQuery, BrandAdjustSummary>,
    IGetService<AdjustPlayerSummaryQuery, int>
{
    public IAsyncEnumerable<BrandAdjustSummary> QueryAsync(AdjustPlayerSummaryQuery query, CancellationToken cancellationToken = default)
        => tgsRepository.QueryPlayerAdjustSummariesAsync(
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.PlayerCode,
                query.Operator),
            cancellationToken);

    public ValueTask<int> GetAsync(AdjustPlayerSummaryQuery query, CancellationToken cancellationToken = default)
        => tgsRepository.CountQueryPlayerAdjustSummariesAsync(
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.PlayerCode,
                query.Operator),
            cancellationToken);
}
