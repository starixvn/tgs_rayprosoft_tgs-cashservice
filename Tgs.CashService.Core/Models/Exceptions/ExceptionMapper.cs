namespace Tgs.CashService.Models
{
    public static class ExceptionMapper
    {
        public static readonly Dictionary<Type, string> ErrorCodeByException = new()
        {
            [typeof(DuplicateTransactionException)] = ErrorCode.DuplicateTransaction,
            [typeof(NotFoundWalletException)] = ErrorCode.NotFoundWallet,
            [typeof(InsufficientFundException)] = ErrorCode.InsufficientFunds,
            [typeof(InvalidDataException)] = ErrorCode.InvalidData,
        };

        public static string GetErrorCodeByException(Exception ex)
        {
            var type = ex.GetType();
            return ErrorCodeByException.TryGetValue(type, out var errorCode) ? errorCode : ErrorCode.Undefined;
        }
    }
}
