namespace Tgs.CashService.Models
{
    public class DuplicateTransactionByActionException(string? message) : Exception(message)
    {
    }
}
