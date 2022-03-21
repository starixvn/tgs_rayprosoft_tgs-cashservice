namespace Tgs.CashService.ApplicationServices;

public record GameSettleLoseCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
