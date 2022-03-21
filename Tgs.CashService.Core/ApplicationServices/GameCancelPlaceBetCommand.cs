namespace Tgs.CashService.ApplicationServices;

public record GameCancelPlaceBetCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
