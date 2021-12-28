using System;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record AllAgentRebateCommand(
        Guid WalletId,
        decimal Amount,
        string? Description,
        string? ExternalId);
}