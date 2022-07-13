namespace Tgs.CashService.ApplicationServices;

public record GameUnlockCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
