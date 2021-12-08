using System;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record DepositCommand(
        Guid WalletId,
        decimal Amount,
        string? Description,
        string? ExternalId,
        Guid? CreatedByUserId);
}
