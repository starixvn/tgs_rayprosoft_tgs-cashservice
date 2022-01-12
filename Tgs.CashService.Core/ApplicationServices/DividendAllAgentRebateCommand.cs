using System;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record DividendAllAgentRebateCommand(
        Guid WalletId,
        decimal Amount,
        string? Description,
        string? ExternalId);
}
