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
        using var cts = HttpContext.RequestAborted.Wait10Seconds();

        var command = new TransferLockCommand(
            walletId,
            viewModel.Amount,
            viewModel.Description,
            viewModel.ExternalId,
            executor);

        var result = await exchangeService
            .GetAsync(command, cts.Token)
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    [HttpPost("receive-lock/{walletId}", Name = "ReceiveLock")]
    public async Task<TransactionResultViewModel> ReceiveLockAsync(
        [FromServices] IGetService<ReceiveLockCommand, ExchangeResult> exchangeService,
        [FromHeader(Name = "Modifier")] Guid executor,
        Guid walletId,
        [FromBody] TransferViewModel viewModel)
    {
        using var cts = HttpContext.RequestAborted.Wait10Seconds();

        var command = new ReceiveLockCommand(
            walletId,
            viewModel.Amount,
            viewModel.Description,
            viewModel.ExternalId,
            executor);

        var result = await exchangeService
            .GetAsync(command, cts.Token)
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
        using var cts = HttpContext.RequestAborted.Wait10Seconds();

        var command = new TransferCommitCommand(
            walletId,
            viewModel.Amount,
            viewModel.Description,
            viewModel.ExternalId,
            executor);

        var result = await exchangeService
            .GetAsync(command, cts.Token)
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    [HttpPut("receive/{walletId}", Name = "ReceiveCommit")]
    public async Task<TransactionResultViewModel> CommitReceiveAsync(
        [FromServices] IGetService<ReceiveCommitCommand, ExchangeResult> exchangeService,
        [FromHeader(Name = "Modifier")] Guid executor,
        Guid walletId,
        [FromBody] TransferViewModel viewModel)
    {
        using var cts = HttpContext.RequestAborted.Wait10Seconds();

        var command = new ReceiveCommitCommand(
            walletId,
            viewModel.Amount,
            viewModel.Description,
            viewModel.ExternalId,
            executor);

        var result = await exchangeService
            .GetAsync(command, cts.Token)
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    [HttpDelete("transfer/{walletId}", Name = "TransferRollback")]
    public async Task<TransactionResultViewModel> RollbackTransferAsync(
        [FromServices] IGetService<TransferRollbackCommand, ExchangeResult> exchangeService,
        [FromHeader(Name = "Modifier")] Guid executor,
        Guid walletId,
        [FromBody] TransferViewModel viewModel)
    {
        using var cts = HttpContext.RequestAborted.Wait10Seconds();

        var command = new TransferRollbackCommand(
            walletId,
            viewModel.Amount,
            viewModel.Description,
            viewModel.ExternalId,
            executor);

        var result = await exchangeService
            .GetAsync(command, cts.Token)
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    [HttpDelete("receive/{walletId}", Name = "ReceiveRollback")]
    public async Task<TransactionResultViewModel> RollbackReceiveAsync(
        [FromServices] IGetService<ReceiveRollbackCommand, ExchangeResult> exchangeService,
        [FromHeader(Name = "Modifier")] Guid executor,
        Guid walletId,
        [FromBody] TransferViewModel viewModel)
    {
        using var cts = HttpContext.RequestAborted.Wait10Seconds();

        var command = new ReceiveRollbackCommand(
            walletId,
            viewModel.Amount,
            viewModel.Description,
            viewModel.ExternalId,
            executor);

        var result = await exchangeService
            .GetAsync(command, cts.Token)
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }
}
