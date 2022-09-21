using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers;

[Route("wallet")]
[ApiController]
public class TransferWalletController : ControllerBase
{
    [HttpPost("transfer-lock/{walletId}", Name = "TransferLock")]
    public async Task<TransactionResultViewModel> TransferLockAsync(
        [FromServices] IGetService<TransferLockCommand, ExchangeResult> exchangeService,
        [FromHeader(Name = "Modifier")] Guid executor,
        Guid walletId,
        [FromBody] TransferViewModel viewModel)
    {
        var command = new TransferLockCommand(
            walletId,
            viewModel.Amount,
            viewModel.LockAmount,
            viewModel.Description,
            viewModel.ExternalId,
            executor);

        var result = await exchangeService
            .GetAsync(command, HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }
}
