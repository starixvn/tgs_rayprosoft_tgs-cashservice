namespace Tgs.CashService.ApplicationServices;

public record AdjustCompanyCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);
