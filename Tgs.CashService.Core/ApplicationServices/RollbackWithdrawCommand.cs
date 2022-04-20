namespace Tgs.CashService.ApplicationServices;
public record RollbackWithdrawCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid? CreatedByUserId);
