using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers;

[Route("wallet")]
[ApiController]
public class GameProviderWalletController : ControllerBase
{
    /// <summary>
    ///     Games the provider deposit exchange asynchronous.
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
    ///     "description": "測試遊戲上分",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("gp/deposit/{walletId}", Name = "GameProviderDeposit")]
    public async ValueTask<TransactionResultViewModel> GameProviderDepositAsync(
        [FromServices] IGetService<GameProviderDepositCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameProviderDepositViewModel request)
    {
        var result = await service
            .GetAsync(new GameProviderDepositCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    ///     Games the provider fund in exchange asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="walletId"></param>
    /// <param name="viewModel">The view model.</param>
    /// <returns></returns>
    /// <remarks>
    ///     <b>Example</b><br />
    ///     Post WalletId: 36a6cdef-094f-eb11-8d88-70b5e82b6b8d
    ///     {
    ///     "amount": 10,
    ///     "description": "測試主錢包轉入(GP->TGS)",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("gp/fund-in/{walletId}", Name = "GameProviderFundIn")]
    public async ValueTask<TransactionResultViewModel> GameProviderFundInAsync(
        [FromServices] IGetService<GameProviderFundInCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameProviderFundInViewModel viewModel)
    {
        var result = await service
            .GetAsync(new GameProviderFundInCommand(
                walletId,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    ///     Games the provider fund out exchange asynchronous.
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
    ///     "description": "測試主錢包轉出(TGS->GP)",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("gp/fund-out/{walletId}", Name = "GameProviderFundOut")]
    public async ValueTask<TransactionResultViewModel> GameProviderFundOutAsync(
        [FromServices] IGetService<GameProviderFundOutCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameProviderFundOutViewModel request)
    {
        var result = await service
            .GetAsync(new GameProviderFundOutCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    /// Games the provider fund refund asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="walletId">The wallet identifier.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <remarks>
    ///     <b>Example</b><br />
    ///     Post WalletId: 36a6cdef-094f-eb11-8d88-70b5e82b6b8d
    ///     {
    ///     "amount": 10,
    ///     "description": "測試退款機制",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("gp/fund-refund/{walletId}", Name = "GameProviderFundRefund")]
    public async ValueTask<TransactionResultViewModel> GameProviderFundRefundAsync(
        [FromServices] IGetService<GameProviderFundRefundCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameProviderFundOutViewModel request)
    {
        var result = await service
            .GetAsync(new GameProviderFundRefundCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    ///     Games the provider withdrawal exchange asynchronous.
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
    ///     "description": "測試遊戲下分",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("gp/withdrawal/{walletId}", Name = "GameProviderWithdrawal")]
    public async ValueTask<TransactionResultViewModel> GameProviderWithdrawalAsync(
        [FromServices] IGetService<GameProviderWithdrawalCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameProviderWithdrawalViewModel request)
    {
        var result = await service
            .GetAsync(new GameProviderWithdrawalCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }
}
