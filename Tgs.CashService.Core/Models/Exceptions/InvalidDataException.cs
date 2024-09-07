namespace Tgs.CashService.Models
{
    public class InvalidDataException(string? message) : Exception(message)
    {
    }
}
