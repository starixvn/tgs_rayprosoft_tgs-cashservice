namespace Tgs.CashService.ApplicationServices;

public record AdjustBonusCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);
