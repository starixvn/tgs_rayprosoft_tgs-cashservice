namespace Tgs.CashService.ApplicationServices;

public record GameProviderDepositCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
