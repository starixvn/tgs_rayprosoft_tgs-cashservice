namespace Tgs.CashQueryService;

public interface ITgsRepository
{
    IAsyncEnumerable<AdjustRecord> QueryPlayerAdjustRecordsAsync(
        AdjustRecordQueryCondition condition,
        int take,
        int skip = 0,
        CancellationToken cancellationToken = default);
}
