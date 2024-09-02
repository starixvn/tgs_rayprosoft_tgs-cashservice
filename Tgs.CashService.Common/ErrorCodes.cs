namespace Tgs.CashService;

public enum ErrorCodes
{
    Invalid = -1,
    Success = 0,
    DuplicateTransaction = 1,
    NotFoundWallet = 2,
    InvalidAmount = 307,
    InsufficientFunds = 502,
}

public static class ErrorMessages
{
    public static readonly Dictionary<ErrorCodes, string> Messages = new()
    {
        [ErrorCodes.Success] = "",
        [ErrorCodes.DuplicateTransaction] = "Duplicate Transaction",
        [ErrorCodes.NotFoundWallet] ="Can't found wallet!",
        [ErrorCodes.InsufficientFunds] = "Player Has Insufficient Funds",
        [ErrorCodes.InvalidAmount] = "Invalid Amount",
    };
}