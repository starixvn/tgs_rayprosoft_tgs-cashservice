namespace Tgs.CashService.Models
{
    public record ExchangeWalletRecord(Guid? WalletId, 
        TransactionType TransactionType,
        decimal Amount,
        decimal LockAmount,
        string Description,
        string ExternalId,
        Guid? UserId = null,
        string PlayerCode = "",
        bool IsAllowNegative = true,
        string? ProviderCode = null,
        string? OperationId = null,
        string? Action = null);
}
