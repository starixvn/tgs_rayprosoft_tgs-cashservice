namespace Tgs.CashService.Models
{
    public class InsufficientFundException(string? message) : Exception(message)
    {
    }
}
