using Tgs.CashService;

namespace Tgs.CashServiceWorker;

internal record ExchangeRequest(
    Guid WalletId,
    TransactionType TransactionType,
    decimal Amount,
    decimal LockAmount,
    string? Description,
    string? ExternalId,
    Guid? CreatedByUserId = null);
