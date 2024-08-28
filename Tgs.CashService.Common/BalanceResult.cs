namespace Tgs.CashService;

public record BalanceResult(
    Guid TransactionId,
    decimal Balance,
    decimal LockedBalance,
    int ErrorCode,
    string ErrorMessage);
