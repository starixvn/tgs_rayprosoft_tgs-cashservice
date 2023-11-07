namespace Tgs.CashService.ApplicationServices;
public record TransferRollbackCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    Guid ExternalId,
    Guid? Executor);
