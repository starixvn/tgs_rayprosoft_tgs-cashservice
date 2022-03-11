namespace Tgs.CashService.ApplicationServices;

public record CompanyAdjustCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);