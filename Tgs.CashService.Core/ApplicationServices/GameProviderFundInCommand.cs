using System;

namespace Tgs.CashService.Core.ApplicationServices;

public record GameProviderFundInCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
