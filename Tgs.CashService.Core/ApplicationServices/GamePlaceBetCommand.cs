namespace Tgs.CashService.ApplicationServices;

public record GamePlaceBetCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
