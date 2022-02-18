namespace Tgs.CashService.ApplicationServices;

public record GameProviderWithdrawalCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
