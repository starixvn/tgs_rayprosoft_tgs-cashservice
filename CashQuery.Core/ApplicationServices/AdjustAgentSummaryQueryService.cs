using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustAgentSummaryQueryService(ITgsRepository tgsRepository)
    : IQueryService<AdjustAgentSummaryQuery, BrandAdjustSummary>
    , IGetService<AdjustAgentSummaryQuery, int>
{
    public IAsyncEnumerable<BrandAdjustSummary> QueryAsync(AdjustAgentSummaryQuery query, CancellationToken cancellationToken = default)
        => tgsRepository.QueryAgentAdjustSummariesAsync(
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.AgentId?.ToString(),
                query.Operator),
            cancellationToken);

    public ValueTask<int> GetAsync(AdjustAgentSummaryQuery query, CancellationToken cancellationToken = default)
        => tgsRepository.CountQueryAgentAdjustSummariesAsync(
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.AgentId?.ToString(),
                query.Operator),
            cancellationToken);
}
