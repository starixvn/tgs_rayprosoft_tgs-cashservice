
using Tgs.Cash.Common;

namespace Tgs.CashQueryService.ApplicationServices;
public record AdjustPlayerWalletRecordQuery(
    Guid BrandId,
    ValueRange<DateTime> CreatedOnRange,
    IEnumerable<AdjustType>? SearchTypes,
    string? UserId,
    string? Operator,
    int Take,
    int Skip = 0);
