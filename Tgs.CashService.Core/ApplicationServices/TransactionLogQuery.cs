using Tgs.Cash.Common;
using Tgs.Paging;

namespace Tgs.CashService.ApplicationServices;
public record TransactionLogQuery(
    Guid WalletId,
    ValueRange<DateTime> QueryRange,
    IEnumerable<TransactionType> QueryTypes,
    Paging<TransactionLog> Paging);
