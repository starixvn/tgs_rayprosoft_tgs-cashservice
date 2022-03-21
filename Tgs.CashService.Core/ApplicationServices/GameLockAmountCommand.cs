namespace Tgs.CashService.ApplicationServices;

public record GameLockAmountCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
