namespace Tgs.CashService.ApplicationServices;

public record GameProviderCancelSettlementCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string ExternalId);
