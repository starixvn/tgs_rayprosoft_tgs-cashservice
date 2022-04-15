namespace Tgs.CashService.ApplicationServices;

public record GameSettleWinCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
