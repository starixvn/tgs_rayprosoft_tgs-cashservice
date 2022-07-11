namespace Tgs.CashService.ApplicationServices;

public record GameLockCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
