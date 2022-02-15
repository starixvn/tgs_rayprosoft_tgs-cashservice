namespace Tgs.CashService.ApplicationServices;

public record PromotionVipLevelSeasonCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
