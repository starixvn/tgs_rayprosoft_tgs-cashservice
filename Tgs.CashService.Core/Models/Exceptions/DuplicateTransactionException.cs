namespace Tgs.CashService.Models
{
    public class DuplicateTransactionException(string? message) : Exception(message)
    {
    }
}
