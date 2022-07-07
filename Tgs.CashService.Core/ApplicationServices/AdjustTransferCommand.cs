namespace Tgs.CashService.ApplicationServices;

public record AdjustTransferCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);
