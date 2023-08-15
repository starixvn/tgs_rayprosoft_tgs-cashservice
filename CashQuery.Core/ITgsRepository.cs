namespace Tgs.CashQueryService;

public interface ITgsRepository
{
    IAsyncEnumerable<AdjustRecord> QueryPlayerAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int skip = 0,
        CancellationToken cancellationToken = default);

    ValueTask<int> CountQueryPlayerAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<AdjustRecord> QueryAgentAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int skip = 0,
        CancellationToken ctancellationToken = default);

    ValueTask<int> CountQueryAgentAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BrandAdjustSummary> QueryPlayerAdjustSummariesAsync(
        AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default);

    ValueTask<int> CountQueryPlayerAdjustSummariesAsync(
        AdjustRecordQueryCondition condition,
        CancellationToken cancellation = default);

    IAsyncEnumerable<BrandAdjustSummary> QueryAgentAdjustSummariesAsync(
        AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default);

    ValueTask<int> CountQueryAgentAdjustSummariesAsync(
        AdjustRecordQueryCondition condition,
        CancellationToken cancellation = default);

    IAsyncEnumerable<PlayerAdjustRecord> QueryPlayerAdjustNetProfitAsync(
        IEnumerable<Guid> transactionIds,
        CancellationToken cancellationToken = default);
}
