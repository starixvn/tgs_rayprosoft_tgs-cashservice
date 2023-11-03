using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers;

[Route("wallet-mgr")]
[ApiController]
public class WalletManagementController : ControllerBase
{
    [HttpPut(Name = "CreateNewCash")]
    public ValueTask<Guid> CreateNewCashAsync(
        [FromServices] IGetService<NewCashCommand, Guid> createService,
        [FromBody] NewCashReqeustViewModel viewModel)
    {
        var command = new NewCashCommand(viewModel.UserId, viewModel.CurrencyCode);

        return createService.GetAsync(command, HttpContext.RequestAborted);
    }
}
