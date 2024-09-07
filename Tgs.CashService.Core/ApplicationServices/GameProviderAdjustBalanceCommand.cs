namespace Tgs.CashService.ApplicationServices;

public record GameProviderAdjustBalanceCommand(
    string PlayerCode,
    decimal AddedAmount,
    decimal DeductedAmount,
    string? Description,
    string? ProviderCode,
    string? ExternalId,
    string? OperationId,
    string? Action);
