namespace Tgs.CashService.ApplicationServices;

public record GameProviderSettleWinCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
