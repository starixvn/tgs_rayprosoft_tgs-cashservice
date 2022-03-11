namespace Tgs.CashService.ApplicationServices;

public record TransferAdjustCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);