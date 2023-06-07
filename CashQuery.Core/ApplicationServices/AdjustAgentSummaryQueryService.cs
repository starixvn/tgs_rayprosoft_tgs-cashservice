using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustAgentSummaryQueryService : IQueryService<AdjustAgentSummaryQuery, BrandAdjustSummary>
{
    private readonly ITgsRepository _tgsRepository;

    public AdjustAgentSummaryQueryService(ITgsRepository tgsRepository)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
    }

    public IAsyncEnumerable<BrandAdjustSummary> QueryAsync(AdjustAgentSummaryQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.QueryAgentAdjustSummariesAsync(
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.AgentId?.ToString(),
                query.Operator),
            cancellationToken);
}
