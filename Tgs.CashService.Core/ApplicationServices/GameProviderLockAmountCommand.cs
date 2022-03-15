namespace Tgs.CashService.ApplicationServices;

public record GameProviderLockAmountCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string ExternalId);
