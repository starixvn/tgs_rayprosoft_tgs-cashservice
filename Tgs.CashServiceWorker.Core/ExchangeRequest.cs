namespace Tgs.CashService.Core;

internal record ExchangeRequest(
    Guid WalletId,
    TransactionType TransactionType,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid? CreatedByUserId = null);
