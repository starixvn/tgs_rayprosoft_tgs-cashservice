namespace Tgs.CashService.ApplicationServices;

public record AdjustCompanyV2Command(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);
