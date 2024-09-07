namespace Tgs.CashService.ApplicationServices;

public record GameProviderAddBalanceCommand(
    string PlayerCode,
    decimal Amount,
    string? Description,
    string? ProviderCode,
    string? ExternalId,
    string? OperationId,
    string? Action);
