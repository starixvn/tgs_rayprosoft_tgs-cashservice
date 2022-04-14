namespace Tgs.CashService.ApplicationServices;

public record PromotionVipLevelBirthdayCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
