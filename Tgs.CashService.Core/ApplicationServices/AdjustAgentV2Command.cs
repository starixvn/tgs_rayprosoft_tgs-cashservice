namespace Tgs.CashService.ApplicationServices;

public record AdjustAgentV2Command(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid CreatedByUserId);