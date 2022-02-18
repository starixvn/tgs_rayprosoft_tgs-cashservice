namespace Tgs.CashService.ApplicationServices;

public record BonusCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
