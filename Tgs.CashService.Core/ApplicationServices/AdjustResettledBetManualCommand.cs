namespace Tgs.CashService.ApplicationServices;

public record AdjustResettledBetManualCommand(
   Guid WalletId,
   decimal Amount,
   string Description,
   string ExternalId,
   Guid CreatedByUserId);
