namespace Tgs.CashService.ApplicationServices;

public record GameUnlockAmountCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
