using Tgs.Cash.Common;

namespace Tgs.CashService.ApplicationServices;
public record TransactionLogQuery(
    Guid WalletId,
    ValueRange<DateTime> QueryRange,
    IEnumerable<TransactionType> QueryTypes,
    int PageIndex,
    int PageSize);
