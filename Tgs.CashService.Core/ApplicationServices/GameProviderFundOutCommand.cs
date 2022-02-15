namespace Tgs.CashService.ApplicationServices;

public record GameProviderFundOutCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
