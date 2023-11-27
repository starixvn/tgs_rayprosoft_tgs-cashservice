using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustAgentSummaryQueryService(ITgsRepository tgsRepository)
    : IQueryService<AdjustAgentSummaryQuery, BrandAdjustSummary>
    , IGetService<AdjustAgentSummaryQuery, int>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository
        ?? throw new ArgumentNullException(nameof(tgsRepository));

    public IAsyncEnumerable<BrandAdjustSummary> QueryAsync(AdjustAgentSummaryQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.QueryAgentAdjustSummariesAsync(
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.AgentId?.ToString(),
                query.Operator),
            cancellationToken);

    public ValueTask<int> GetAsync(AdjustAgentSummaryQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.CountQueryAgentAdjustSummariesAsync(
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.AgentId?.ToString(),
                query.Operator),
            cancellationToken);
}
