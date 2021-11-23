using System;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record OtherCommand(
        Guid WalletId,
        decimal Amount,
        string Description,
        string ExternalId,
        Guid CreatedByUserId);
}
