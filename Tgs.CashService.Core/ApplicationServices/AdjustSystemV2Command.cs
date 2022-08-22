namespace Tgs.CashService.ApplicationServices;

public record AdjustSystemV2Command(
    Guid WalletId,
    decimal Amount,
    decimal LockAmount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);
