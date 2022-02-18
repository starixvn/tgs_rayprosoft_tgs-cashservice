namespace Tgs.CashService.ApplicationServices;

public record PromotionCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid? CreateByUserId);
