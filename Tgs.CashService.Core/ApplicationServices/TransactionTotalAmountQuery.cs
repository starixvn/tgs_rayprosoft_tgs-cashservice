using Tgs.Cash.Common;

namespace Tgs.CashService.ApplicationServices;
public record TransactionTotalAmountQuery(
    Guid WalletId,
    ValueRange<DateTime> QueryRange,
    IEnumerable<TransactionType> QueryTypes);
