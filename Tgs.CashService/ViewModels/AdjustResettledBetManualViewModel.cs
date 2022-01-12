using System;

namespace Tgs.CashService.ViewModels
{
    public record AdjustResettledBetManualViewModel(
       decimal Amount,
       string Description,
       string ExternalId,
       Guid CreatedByUserId);
}
