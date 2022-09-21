namespace Tgs.CashService.ApplicationServices;
public record TransferLockCommand(
    Guid WalletId,
    decimal Amount,
    decimal LockAmount,
    string? Description,
    Guid ExternalId,
    Guid Executor);
