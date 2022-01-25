using System;

namespace Tgs.CashService.Core.ApplicationServices;

public record GameProviderWithdrawalCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
