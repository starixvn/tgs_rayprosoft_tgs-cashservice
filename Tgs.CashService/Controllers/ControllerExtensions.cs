namespace Tgs.CashService.Controllers;

public static class ControllerExtensions
{
    public static CancellationTokenSource WithTimeout(this CancellationToken token, TimeSpan timeout)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        cts.CancelAfter(timeout);

        return cts;
    }

    public static CancellationTokenSource Wait10Seconds(this CancellationToken token)
        => token.WithTimeout(TimeSpan.FromSeconds(10D));
}
