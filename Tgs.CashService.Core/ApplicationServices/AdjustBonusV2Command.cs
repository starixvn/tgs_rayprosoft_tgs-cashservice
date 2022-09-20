namespace Tgs.CashService.ApplicationServices;

public record AdjustBonusV2Command(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);