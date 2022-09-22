using Microsoft.AspNetCore.Mvc;

namespace Tgs.CashService.Controllers;

public static class ControllerExtensions
{
    public static CancellationToken CreateCancellationToken(this ControllerBase controller)
        => controller.HttpContext.RequestAborted
            .WithTimeout(TimeSpan.FromSeconds(10D));

    public static CancellationToken WithTimeout(this CancellationToken token, TimeSpan timeout)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        cts.CancelAfter(timeout);

        return cts.Token;
    }

    public static CancellationToken Wait10Seconds(this CancellationToken token)
        => token.WithTimeout(TimeSpan.FromSeconds(10D));
}
