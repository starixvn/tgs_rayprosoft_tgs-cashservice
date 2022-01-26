namespace Tgs.CashService.Core.ApplicationServices;

public record CancelWithdrawalCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid? CreatedByUserId);
