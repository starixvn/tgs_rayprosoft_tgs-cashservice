using System;

namespace Tgs.CashService.Core.ApplicationServices;

public record PromotionCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid? CreateByUserId);
