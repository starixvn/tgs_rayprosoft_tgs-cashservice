namespace Tgs.CashService.ApplicationServices;
public record TransferLockCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    Guid ExternalId,
    Guid? Executor);
