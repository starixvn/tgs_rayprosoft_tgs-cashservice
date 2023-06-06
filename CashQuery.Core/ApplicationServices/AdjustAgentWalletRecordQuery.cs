using Tgs.Cash.Common;

namespace Tgs.CashQueryService.ApplicationServices;
public record AdjustAgentWalletRecordQuery(
    Guid BrandId,
    ValueRange<DateTime> CreatedOnRange,
    IEnumerable<AdjustType>? SearchTypes,
    Guid? AgentId,
    string? Operator,
    int Take,
    int Skip = 0);
