namespace Tgs.CashService.ApplicationServices;

public record AdjustSystemCommand(
    Guid WalletId,
    decimal Amount,
    decimal LockAmount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);