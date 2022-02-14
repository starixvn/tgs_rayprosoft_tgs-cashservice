namespace Tgs.CashServiceWorker;

public class RequestInvalidSubjectException : Exception
{
    public RequestInvalidSubjectException(string? message) : base(message)
    {
    }
}
