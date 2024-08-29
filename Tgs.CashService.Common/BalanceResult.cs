
namespace Tgs.CashService;

public record BalanceResult(
    Guid TransactionId,
    decimal Balance,
    decimal LockedBalance,
    ErrorCodes ErrorCode = ErrorCodes.Success)
{
    public string ErrorMessage => ErrorMessages.Messages[ErrorCode];
};
