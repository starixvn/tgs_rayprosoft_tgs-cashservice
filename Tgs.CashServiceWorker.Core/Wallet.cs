namespace Tgs.CashServiceWorker;

public record Wallet(
    Guid Id,
    Guid BrandId,
    string UserId,
    decimal Balance,
    decimal LockedBalance,
    Guid? SnapshotId);
