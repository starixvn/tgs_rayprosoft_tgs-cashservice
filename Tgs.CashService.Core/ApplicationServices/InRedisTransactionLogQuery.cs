using Tgs.Paging;

namespace Tgs.CashService.ApplicationServices;
public record InRedisTransactionLogQuery(
    Guid WalletId,
    Paging<TransactionLog> Paging);
