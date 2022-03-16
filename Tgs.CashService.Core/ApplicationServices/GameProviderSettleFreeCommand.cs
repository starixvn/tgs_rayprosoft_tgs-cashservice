namespace Tgs.CashService.ApplicationServices;

public record GameProviderSettleFreeCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
