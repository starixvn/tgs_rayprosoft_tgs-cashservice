namespace Tgs.CashService.ApplicationServices;

public record GameProviderUnlockAmountCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
