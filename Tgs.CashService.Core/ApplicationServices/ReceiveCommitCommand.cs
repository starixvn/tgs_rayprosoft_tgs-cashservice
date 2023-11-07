namespace Tgs.CashService.ApplicationServices;
public record ReceiveCommitCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    Guid ExternalId,
    Guid? Executor);
