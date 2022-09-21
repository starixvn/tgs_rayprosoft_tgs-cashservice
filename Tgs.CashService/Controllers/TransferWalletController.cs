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
            viewModel.Description,
            viewModel.ExternalId,
            executor);

        var result = await exchangeService
            .GetAsync(command, this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    [HttpPut("transfer/{walletId}", Name = "TransferCommit")]
    public async Task<TransactionResultViewModel> CommitTransferAsync(
        [FromServices] IGetService<TransferCommitCommand, ExchangeResult> exchangeService,
        [FromHeader(Name = "Modifier")] Guid executor,
        Guid walletId,
        [FromBody] TransferViewModel viewModel)
    {
        var command = new TransferCommitCommand(
            walletId,
            viewModel.Amount,
            viewModel.Description,
            viewModel.ExternalId,
            executor);

        var result = await exchangeService
            .GetAsync(command, this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }
}
