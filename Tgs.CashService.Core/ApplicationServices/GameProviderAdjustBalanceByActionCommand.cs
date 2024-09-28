namespace Tgs.CashService.ApplicationServices;

public record GameProviderAdjustBalanceByActionCommand(
    string PlayerCode,
    decimal Amount,
    string? Description,
    string ProviderCode,
    string ExternalId,
    string OperationId,
    string Action,
    bool IsAllowNegative);
