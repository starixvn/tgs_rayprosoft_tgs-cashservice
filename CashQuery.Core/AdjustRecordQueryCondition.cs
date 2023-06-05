using Tgs.Cash.Common;

namespace Tgs.CashQueryService;
public record AdjustRecordQueryCondition(
    Guid BrandId,
    ValueRange<DateTime> CreatedOnRange,
    IEnumerable<AdjustType>? SearchTypes,
    string? UserId,
    string? Operator);
