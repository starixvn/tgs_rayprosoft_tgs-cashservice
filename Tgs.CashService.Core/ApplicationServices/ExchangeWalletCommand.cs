namespace Tgs.CashService.Core.ApplicationServices;

public record ExchangeWalletCommand(
    Guid WalletId,
    TransactionType TransactionType,
    Guid BrandId,
    string PlayerCode,
    decimal Amount,
    string? Description,
    string? ExternalId,
    Guid? CreatedByUserId);
