namespace Tgs.CashService.ApplicationServices;

public record AdjustAgentCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);
