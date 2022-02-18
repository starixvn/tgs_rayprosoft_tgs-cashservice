using Tgs.CashService;

namespace Tgs.CashServiceWorker.ApplicationServices;

public record ExchangeWalletCommand(
    Guid WalletId,
    TransactionType TransactionType,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid? CreatedByUserId);
