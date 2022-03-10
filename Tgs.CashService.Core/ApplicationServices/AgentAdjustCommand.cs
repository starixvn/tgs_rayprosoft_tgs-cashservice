namespace Tgs.CashService.ApplicationServices;

public record AgentAdjustCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);