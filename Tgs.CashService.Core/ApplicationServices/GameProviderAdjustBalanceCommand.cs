namespace Tgs.CashService.ApplicationServices;

public record GameProviderAdjustBalanceCommand(
    string PlayerCode,
    decimal AddedAmount,
    decimal DeductedAmount,
    Guid? WalletId,
    string? Description,
    string? ExternalId);
