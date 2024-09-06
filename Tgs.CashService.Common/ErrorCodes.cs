namespace Tgs.CashService;

public static class ErrorCode
{
    public const string Success = "0";
    public const string Undefined = "201";
    public const string DuplicateTransaction = "202";
    public const string NotFoundWallet = "203";
    public const string InvalidAmount = "204";
    public const string InsufficientFunds = "205";
}

public static class ErrorMessages
{
    public static readonly Dictionary<string, string> Messages = new()
    {
        [ErrorCode.Success] = "",
        [ErrorCode.Undefined] = "Internal Server Error",
        [ErrorCode.DuplicateTransaction] = "Duplicate Transaction",
        [ErrorCode.NotFoundWallet] ="Can't found wallet",
        [ErrorCode.InsufficientFunds] = "Player Has Insufficient Funds",
        [ErrorCode.InvalidAmount] = "Invalid Amount",
    };
}