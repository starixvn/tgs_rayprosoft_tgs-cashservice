namespace Tgs.CashService.ApplicationServices;

public record GameCancelTransactionCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
