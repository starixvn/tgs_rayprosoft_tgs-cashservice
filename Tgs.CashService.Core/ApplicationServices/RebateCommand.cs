namespace Tgs.CashService.Core.ApplicationServices;

public record RebateCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
