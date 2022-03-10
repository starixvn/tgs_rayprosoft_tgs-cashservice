namespace Tgs.CashService.ApplicationServices;

public record SystemAdjustCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);