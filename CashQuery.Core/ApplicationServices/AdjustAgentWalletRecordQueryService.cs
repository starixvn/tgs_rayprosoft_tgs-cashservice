using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustAgentWalletRecordQueryService
    : IQueryService<AdjustAgentWalletRecordQuery, AdjustRecord>,
    IGetService<AdjustAgentWalletRecordQuery, int>
{
    private readonly ITgsRepository _tgsRepository;

    public AdjustAgentWalletRecordQueryService(ITgsRepository tgsRepository)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
    }

    public IAsyncEnumerable<AdjustRecord> QueryAsync(AdjustAgentWalletRecordQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.QueryAgentAdjustRecordsAsync(
            query.BrandId,
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.AgentId?.ToString(),
                query.Operator),
            query.Skip,
            cancellationToken);

    public ValueTask<int> GetAsync(AdjustAgentWalletRecordQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.CountQueryAgentAdjustRecordsAsync(
            query.BrandId,
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.AgentId?.ToString(),
                query.Operator),
            cancellationToken);
}
