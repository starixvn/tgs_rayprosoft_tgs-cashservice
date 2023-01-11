namespace Tgs.CashService;
public record TransactionLog(
    Guid Id,
    DateTime RecordedOn,
    TransactionType Type,
    decimal Amount,
    decimal Balance,
    decimal LockAmount,
    decimal LockedBalance,
    string? Description,
    string? ExternalId);
