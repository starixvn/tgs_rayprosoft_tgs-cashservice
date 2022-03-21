namespace Tgs.CashService.ApplicationServices;

public record GameCancelSettlementCommand(
    Guid WalletId,
    decimal Amount,
    string ExternalId);
