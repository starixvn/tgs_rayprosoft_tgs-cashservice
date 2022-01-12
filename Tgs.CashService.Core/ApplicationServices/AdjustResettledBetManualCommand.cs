using System;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record AdjustResettledBetManualCommand(
       Guid WalletId,
       decimal Amount,
       string Description,
       string ExternalId,
       Guid CreatedByUserId);
}
