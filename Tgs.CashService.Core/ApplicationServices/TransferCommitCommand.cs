namespace Tgs.CashService.ApplicationServices;
public record TransferCommitCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    Guid ExternalId,
    Guid Executor);
