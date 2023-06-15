namespace Tgs.CashQueryService.Client;

public interface ICashQueryClient
{
    ValueTask<int> CountAgentAdjustRecordsAsync(Guid brandId, AdjustRecordQueryCondition condition, CancellationToken cancellationToken = default);

    IAsyncEnumerable<AdjustRecordListItem> QueryAgentAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int totalCount,
        int skip = 0,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<AdjustSummaryListItem> QueryAgentAdjustSummariesAsync(AdjustRecordQueryCondition condition, CancellationToken cancellationToken = default);

    ValueTask<int> CountPlayerAdjustRecordsAsync(Guid brandId, AdjustRecordQueryCondition condition, CancellationToken cancellationToken = default);

    IAsyncEnumerable<AdjustRecordListItem> QueryPlayerAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int totalCount,
        int skip = 0,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<AdjustSummaryListItem> QueryPlayerAdjustSummariesAsync(AdjustRecordQueryCondition condition, CancellationToken cancellationToken = default);
}
