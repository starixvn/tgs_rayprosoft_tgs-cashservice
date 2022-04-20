namespace Tgs.CashService.ApplicationServices;
public record WithdrawLockCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
