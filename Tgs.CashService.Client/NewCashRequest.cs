namespace Tgs.CashService.Client;
record NewCashRequest(
    Guid BrandId,
    string UserId,
    string CurrencyCode,
    CashType Type);
