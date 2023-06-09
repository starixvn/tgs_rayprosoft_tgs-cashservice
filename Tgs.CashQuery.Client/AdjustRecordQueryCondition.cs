using Tgs.Cash.Common;
using Tgs.CashQueryService;

namespace Tgs.CashQueryService.Client;
public record AdjustRecordQueryCondition(
    ValueRange<DateTime> CreatedOnRange,
    IEnumerable<AdjustType>? SearchTypes,
    string? UserId,
    string? Operator);
