using System;

namespace Tgs.CashService.ViewModels
{
    public record ResettledBetManualViewModel(
       Guid WalletId,
       decimal Amount,
       string Description,
       string ExternalId,
       Guid CreatedByUserId);
}
