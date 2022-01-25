using System;

namespace Tgs.CashService.Core.ApplicationServices;

public record BonusCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
