namespace Tgs.CashService.ApplicationServices;

public record AdjustCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);
