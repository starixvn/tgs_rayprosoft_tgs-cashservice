using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustAgentWalletRecordQueryService : IQueryService<AdjustAgentWalletRecordQuery, AdjustRecord>
{
    private readonly ITgsRepository _tgsRepository;

    public AdjustAgentWalletRecordQueryService(ITgsRepository tgsRepository)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
    }

    public IAsyncEnumerable<AdjustRecord> QueryAsync(AdjustAgentWalletRecordQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.QueryAgentAdjustRecordsAsync(
            new AdjustRecordQueryCondition(
                query.BrandId,
                query.CreatedOnRange,
                query.SearchTypes,
                query.AgentId?.ToString(),
                query.Operator),
            query.Take,
            query.Skip,
            cancellationToken);
}
