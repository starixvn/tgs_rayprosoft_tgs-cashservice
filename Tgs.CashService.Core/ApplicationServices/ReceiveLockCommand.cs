namespace Tgs.CashService.ApplicationServices;
public record ReceiveLockCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    Guid ExternalId,
    Guid Executor);
