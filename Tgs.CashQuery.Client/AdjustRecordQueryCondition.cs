using Tgs.Cash.Common;
using Tgs.CashQueryService;

namespace Tgs.CashQuery.Client;
public record AdjustRecordQueryCondition(
    ValueRange<DateTime> CreatedOnRange,
    IEnumerable<AdjustType>? SearchTypes,
    string? UserId,
    string? Operator);
