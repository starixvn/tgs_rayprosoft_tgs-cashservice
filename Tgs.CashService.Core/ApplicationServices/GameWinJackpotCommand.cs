namespace Tgs.CashService.ApplicationServices;

public record GameWinJackpotCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
