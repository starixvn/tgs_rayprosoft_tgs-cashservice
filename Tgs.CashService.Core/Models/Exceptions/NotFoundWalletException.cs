namespace Tgs.CashService.Models
{
    public class NotFoundWalletException(string? message) : Exception(message)
    {
    }
}
