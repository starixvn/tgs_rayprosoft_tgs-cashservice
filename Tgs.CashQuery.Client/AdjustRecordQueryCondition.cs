using Tgs.Cash.Common;

namespace Tgs.CashQueryService.Client;
public record AdjustRecordQueryCondition(
    ValueRange<DateTime> CreatedOnRange,
    IEnumerable<AdjustType>? SearchTypes,
    string? UserId,
    string? Operator);
