namespace Tgs.CashService.ApplicationServices;

public record GameProviderDeductBalanceCommand(
    string PlayerCode,
    decimal Amount,
    Guid? WalletId,
    string? Description,
    string? ExternalId);
