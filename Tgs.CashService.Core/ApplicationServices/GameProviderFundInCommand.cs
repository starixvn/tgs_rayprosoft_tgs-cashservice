namespace Tgs.CashService.ApplicationServices;

public record GameProviderFundInCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
