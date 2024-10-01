using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustAgentWalletRecordQueryService(ITgsRepository tgsRepository)
        : IQueryService<AdjustAgentWalletRecordQuery, AdjustRecord>,
    IGetService<AdjustAgentWalletRecordQuery, int>
{
    public IAsyncEnumerable<AdjustRecord> QueryAsync(AdjustAgentWalletRecordQuery query, CancellationToken cancellationToken = default)
        => tgsRepository.QueryAgentAdjustRecordsAsync(
            query.BrandId,
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.AgentId?.ToString(),
                query.Operator),
            query.Skip,
            cancellationToken);

    public ValueTask<int> GetAsync(AdjustAgentWalletRecordQuery query, CancellationToken cancellationToken = default)
        => tgsRepository.CountQueryAgentAdjustRecordsAsync(
            query.BrandId,
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.AgentId?.ToString(),
                query.Operator),
            cancellationToken);
}
