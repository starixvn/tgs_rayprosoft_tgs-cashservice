using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers;

[Route("wallet")]
[ApiController]
public class PaymentWalletController : ControllerBase
{
    /// <summary>
    /// Cancels the withdrawal exchange asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="walletId"></param>
    /// <param name="request">The view model.</param>
    /// <returns></returns>
    /// <remarks>
    ///     <b>Example</b><br />
    ///     Post WalletId: 36a6cdef-094f-eb11-8d88-70b5e82b6b8d
    ///     {
    ///     "amount": 10,
    ///     "description": "測試取消提款",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
    ///     }
    /// </remarks>
    [HttpDelete("withdrawal/{walletId}", Name = "CancelWithdrawal")]
    public async ValueTask<TransactionResultViewModel> CancelWithdrawalAsync(
        [FromServices] IGetService<CancelWithdrawalCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] CancelWithdrawalViewModel request)
    {
        var result = await service
            .GetAsync(new CancelWithdrawalCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId,
                request.CreatedByUserId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Deposits the exchange asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="walletId"></param>
    /// <param name="request">The view model.</param>
    /// <returns></returns>
    [HttpPut("deposit/{walletId}", Name = "Deposit")]
    public async ValueTask<TransactionResultViewModel> DepositAsync(
        [FromServices] IGetService<DepositCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] DepositViewModel request)
    {
        var result = await service
            .GetAsync(new DepositCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId,
                request.CreatedByUserId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    /// Deposits the exchange asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="walletId"></param>
    /// <param name="request">The view model.</param>
    /// <returns></returns>
    [HttpPut("deposited/{walletId}", Name = "Deposited")]
    public async ValueTask<TransactionResultViewModel> DepositedAsync(
        [FromServices] IGetService<DepositCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] DepositViewModel request)
    {
        var result = await service
            .GetAsync(new DepositCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId,
                request.CreatedByUserId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    /// Withdrawals the exchange asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="walletId"></param>
    /// <param name="request">The view model.</param>
    /// <returns></returns>
    /// <remarks>
    ///     <b>Example</b><br />
    ///     Post WalletId: 36a6cdef-094f-eb11-8d88-70b5e82b6b8d
    ///     {
    ///     "amount": 10,
    ///     "description": "測試提款",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("withdrawal/{walletId}", Name = "Withdrawal")]
    public async ValueTask<TransactionResultViewModel> WithdrawalAsync(
        [FromServices] IGetService<WithdrawalCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] WithdrawalViewModel request)
    {
        var result = await service
            .GetAsync(new WithdrawalCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }
}
