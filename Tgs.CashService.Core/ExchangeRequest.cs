namespace Tgs.CashService.Core;

internal record ExchangeRequest(
    Guid WalletId,
    TransactionType TransactionType,
    Guid BrandId,
    string PlayerCode,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid? CreatedByUserId = null);
