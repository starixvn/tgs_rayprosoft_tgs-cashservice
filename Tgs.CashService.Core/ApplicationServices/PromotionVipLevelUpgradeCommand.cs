using System;

namespace Tgs.CashService.Core.ApplicationServices;

public record PromotionVipLevelUpgradeCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
