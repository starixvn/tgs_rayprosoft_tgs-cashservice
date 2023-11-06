namespace Tgs.CashService.ApplicationServices;
public record DividendAgentCommand(
    Guid WalletId,
    decimal Amount,
    string? Description,
    string? ExternalId);
