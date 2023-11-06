using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers;

[Route("wallet")]
[ApiController]
public class AgentWalletController : ControllerBase
{
    [HttpPut("agent-dividend/{walletId:Guid}", Name = "Dividend")]
    public async Task<TransactionResultViewModel> DividendAsync(
        [FromServices] IGetService<DividendAgentCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] DividendViewModel viewModel)
    {
        using var cts = HttpContext.RequestAborted.Wait10Seconds();

        var result = await service.GetAsync(
            new DividendAgentCommand(
                walletId,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId),
            cts.Token).ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }
}
