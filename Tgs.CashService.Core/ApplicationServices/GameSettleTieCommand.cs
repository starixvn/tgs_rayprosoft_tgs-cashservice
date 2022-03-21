namespace Tgs.CashService.ApplicationServices;

public record GameSettleTieCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
