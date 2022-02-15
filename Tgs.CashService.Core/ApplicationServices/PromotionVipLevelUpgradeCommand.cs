namespace Tgs.CashService.ApplicationServices;

public record PromotionVipLevelUpgradeCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
