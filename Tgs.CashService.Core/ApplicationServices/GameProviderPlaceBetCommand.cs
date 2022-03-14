namespace Tgs.CashService.ApplicationServices;

public record GameProviderPlaceBetCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string ExternalId);
