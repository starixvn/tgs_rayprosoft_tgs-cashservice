namespace Tgs.CashService.ApplicationServices;
public record CommitWithdrawCommand(
    Guid WalletId,
    decimal Amount,
    decimal ActualAmount,
    string? Description,
    string? ExternalId,
    Guid? CreatedByUserId);
