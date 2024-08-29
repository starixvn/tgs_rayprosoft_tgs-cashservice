namespace Tgs.CashService;

public enum ErrorCodes
{
    Success = 0,
    NotFoundWallet = 1,
    NotEnoughBalance = 2,
    InvalidAmount = 3,
}

public static class ErrorMessages
{
    public static readonly Dictionary<ErrorCodes, string> Messages = new()
    {
        [ErrorCodes.Success] = "",
        [ErrorCodes.NotFoundWallet] ="Can't found wallet!",
        [ErrorCodes.NotEnoughBalance] = "Not enough balance",
        [ErrorCodes.InvalidAmount] = "Invalid Amount",
    };
}