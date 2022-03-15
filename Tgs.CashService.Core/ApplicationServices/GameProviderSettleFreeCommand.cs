namespace Tgs.CashService.ApplicationServices;

public record GameProviderSettleFreeCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string ExternalId);
