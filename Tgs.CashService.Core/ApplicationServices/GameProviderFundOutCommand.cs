using System;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record GameProviderFundOutCommand(
        Guid WalletId,
        decimal Amount,
        string? Description,
        string? ExternalId);
}
