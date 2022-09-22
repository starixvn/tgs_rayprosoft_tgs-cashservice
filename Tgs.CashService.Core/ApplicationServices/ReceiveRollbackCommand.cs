namespace Tgs.CashService.ApplicationServices;
public record ReceiveRollbackCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    Guid ExternalId,
    Guid Executor);
