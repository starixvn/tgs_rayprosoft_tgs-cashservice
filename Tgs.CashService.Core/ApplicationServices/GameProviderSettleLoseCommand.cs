namespace Tgs.CashService.ApplicationServices;

public record GameProviderSettleLoseCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string ExternalId);
