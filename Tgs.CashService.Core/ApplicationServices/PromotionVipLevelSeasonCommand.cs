namespace Tgs.CashService.Core.ApplicationServices;

public record PromotionVipLevelSeasonCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
