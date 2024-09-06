
namespace Tgs.CashService;

public record BalanceResult(
    Guid TransactionId,
    decimal Balance,
    decimal LockedBalance,
    string ErrorCode = ErrorCode.Success)
{
    public string ErrorMessage => ErrorMessages.Messages[ErrorCode];
};
