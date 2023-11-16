using Tgs.CashService;

namespace Tgs.CashServiceWorker;
public record TransactionRecord(
    Guid Id,
    Guid WalletId,
    Guid BrandId,
    string UserId,
    decimal Amount,
    decimal LockAmount,
    decimal Balance,
    decimal LockedBalance,
    TransactionType Type,
    string? Description,
    DateTime RecordedOn,
    string? ExternalId,
    Guid? Modifier);
