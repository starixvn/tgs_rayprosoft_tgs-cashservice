using Tgs.Cash.Common;

namespace Tgs.CashQueryService.ApplicationServices;
public record AdjustPlayerWalletRecordQuery(
    Guid BrandId,
    ValueRange<DateTime> CreatedOnRange,
    IEnumerable<AdjustType>? SearchTypes,
    string? PlayerCode,
    string? Operator,
    int Skip = 0);
