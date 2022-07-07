namespace Tgs.CashService.ApplicationServices;

public record GameBetCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
