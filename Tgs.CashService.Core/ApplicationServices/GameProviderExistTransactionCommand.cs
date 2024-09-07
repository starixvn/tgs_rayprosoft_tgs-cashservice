namespace Tgs.CashService.ApplicationServices;

public record GameProviderExistTransactionCommand(
    string PlayerCode,
    string? ProviderCode,
    string? ExternalId,
    string? Action);
