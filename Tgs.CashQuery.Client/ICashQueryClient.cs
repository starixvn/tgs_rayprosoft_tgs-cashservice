using Tgs.CashQueryService;
using Tgs.CashQueryService.Client;

namespace Tgs.CashQueryService.Client;

public interface ICashQueryClient
{
    IAsyncEnumerable<AdjustRecordListItem> QueryAgentAdjustRecordsAsync(Guid brandId, AdjustRecordQueryCondition condition, int skip = 0, CancellationToken cancellationToken = default);

    IAsyncEnumerable<AdjustSummaryListItem> QueryAgentAdjustSummariesAsync(AdjustRecordQueryCondition condition, CancellationToken cancellationToken = default);

    IAsyncEnumerable<AdjustRecordListItem> QueryPlayerAdjustRecordsAsync(Guid brandId, AdjustRecordQueryCondition condition, int skip = 0, CancellationToken cancellationToken = default);

    IAsyncEnumerable<AdjustSummaryListItem> QueryPlayerAdjustSummariesAsync(AdjustRecordQueryCondition condition, CancellationToken cancellationToken = default);
}
