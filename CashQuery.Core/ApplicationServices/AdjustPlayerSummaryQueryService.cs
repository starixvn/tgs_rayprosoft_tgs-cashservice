using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustPlayerSummaryQueryService
    : IQueryService<AdjustPlayerSummaryQuery, BrandAdjustSummary>,
    IGetService<AdjustPlayerSummaryQuery, int>
{
    private readonly ITgsRepository _tgsRepository;

    public AdjustPlayerSummaryQueryService(ITgsRepository tgsRepository)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
    }

    public IAsyncEnumerable<BrandAdjustSummary> QueryAsync(AdjustPlayerSummaryQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.QueryPlayerAdjustSummariesAsync(
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.PlayerCode,
                query.Operator),
            cancellationToken);

    public ValueTask<int> GetAsync(AdjustPlayerSummaryQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.CountQueryPlayerAdjustSummariesAsync(
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.PlayerCode,
                query.Operator),
            cancellationToken);
}
