namespace Tgs.CashService.ApplicationServices;

public record GameCancelCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
