namespace Tgs.CashQueryService;

public interface ITgsRepository
{
    IAsyncEnumerable<AdjustRecord> QueryPlayerAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int take,
        int skip = 0,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<AdjustRecord> QueryAgentAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int take,
        int skip = 0,
        CancellationToken ctancellationToken = default);

    IAsyncEnumerable<BrandAdjustSummary> QueryPlayerAdjustSummariesAsync(
        AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BrandAdjustSummary> QueryAgentAdjustSummariesAsync(
        AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default);
}
