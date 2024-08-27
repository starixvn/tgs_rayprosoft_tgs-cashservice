namespace Tgs.CashService.ApplicationServices;

public record GameProviderAddBalanceCommand(
    string PlayerCode,
    decimal Amount,
    Guid? WalletId,
    string? Description,
    string? ExternalId);
