namespace Tgs.CashService.ApplicationServices;

public record GameProviderDeductBalanceCommand(
    string PlayerCode,
    decimal Amount,
    string? Description,
    string? ProviderCode,
    string? ExternalId,
    string? OperationId,
    string? Action);
