namespace Tgs.CashService.ApplicationServices;
public record NewCashCommand(
    Guid BrandId,
    string UserId,
    CashType Type,
    string CurrencyCode);
