namespace Tgs.CashService.ApplicationServices;

public record GameProviderCancelPlaceBetCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
