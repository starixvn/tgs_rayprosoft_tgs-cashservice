namespace Tgs.CashService.ApplicationServices;

public record GameProviderSettleTieCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
