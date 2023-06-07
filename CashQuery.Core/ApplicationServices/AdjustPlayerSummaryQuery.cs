using Tgs.Cash.Common;

namespace Tgs.CashQueryService.ApplicationServices;
public record AdjustPlayerSummaryQuery(
    ValueRange<DateTime> CreatedOnRange,
    IEnumerable<AdjustType>? SearchTypes,
    string? PlayerCode,
    string? Operator);
