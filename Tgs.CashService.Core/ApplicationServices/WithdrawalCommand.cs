using System;

namespace Tgs.CashService.Core.ApplicationServices;

public record WithdrawalCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
