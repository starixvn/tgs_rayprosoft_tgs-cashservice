using Microsoft.AspNetCore.Mvc;

namespace Tgs.CashService.Controllers;

public static class ControllerExtensions
{
    public static CancellationToken CreateCancellationToken(this ControllerBase controller)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(controller.HttpContext.RequestAborted);
        cts.CancelAfter(TimeSpan.FromSeconds(10));

        return cts.Token;
    }
}
