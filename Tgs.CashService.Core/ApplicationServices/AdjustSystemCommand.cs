namespace Tgs.CashService.ApplicationServices;

public record AdjustSystemCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);