namespace Tgs.CashService.ApplicationServices;

public record GameSettleFreeCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
