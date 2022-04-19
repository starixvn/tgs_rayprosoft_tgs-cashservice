using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers;

[Route("wallet")]
[ApiController]
public class GameWalletController : ControllerBase
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

    /// <summary>
    ///     Game place bet exchange asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/bet/{walletId}", Name = "GamePlaceBet")]
    public async ValueTask<TransactionResultViewModel> GamePlaceBetAsync(
        [FromServices] IGetService<GamePlaceBetCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GamePlaceBetViewModel request)
    {
        var result = await service
            .GetAsync(new GamePlaceBetCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    ///     Game lock amount exchange asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/lock/{walletId}", Name = "GameLockAmount")]
    public async ValueTask<TransactionResultViewModel> GameLockAmountAsync(
        [FromServices] IGetService<GameLockAmountCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameLockAmountViewModel request)
    {
        var result = await service
            .GetAsync(new GameLockAmountCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    ///     Game unlock amount exchange asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/unlock/{walletId}", Name = "GameUnlockAmount")]
    public async ValueTask<TransactionResultViewModel> GameUnlockAmountAsync(
        [FromServices] IGetService<GameUnlockAmountCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameProviderUnlockAmountViewModel request)
    {
        var result = await service
            .GetAsync(new GameUnlockAmountCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    ///     Game settle win bet exchange asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/settle-win/{walletId}", Name = "GameSettleWin")]
    public async ValueTask<TransactionResultViewModel> GameSettleWinAsync(
        [FromServices] IGetService<GameSettleWinCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameSettleWinViewModel request)
    {
        var result = await service
            .GetAsync(new GameSettleWinCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    ///     Game settle lose bet exchange asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/settle-lose/{walletId}", Name = "GamerSettleLose")]
    public async ValueTask<TransactionResultViewModel> GameSettleLoseAsync(
        [FromServices] IGetService<GameSettleLoseCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameSettleLoseViewModel request)
    {
        var result = await service
            .GetAsync(new GameSettleLoseCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    ///     Game settle tie bet exchange asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/settle-tie/{walletId}", Name = "GameSettleTie")]
    public async ValueTask<TransactionResultViewModel> GameSettleTieAsync(
        [FromServices] IGetService<GameSettleTieCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameSettleTieViewModel request)
    {
        var result = await service
            .GetAsync(new GameSettleTieCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    ///     Game settle free bet exchange asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/settle-free/{walletId}", Name = "GameSettleFree")]
    public async ValueTask<TransactionResultViewModel> GameSettleFreeAsync(
        [FromServices] IGetService<GameSettleFreeCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameSettleFreeViewModel request)
    {
        var result = await service
            .GetAsync(new GameSettleFreeCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    ///     Game cancel transaction exchange asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/cancel/{walletId}", Name = "GameCancelTransaction")]
    public async ValueTask<TransactionResultViewModel> GameCancelTransactionAsync(
        [FromServices] IGetService<GameCancelTransactionCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameCancelTransactionViewModel request)
    {
        var result = await service
            .GetAsync(new GameCancelTransactionCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    ///     Game win jackpot exchange asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/win-jackpot/{walletId}", Name = "GameWinJackpot")]
    public async ValueTask<TransactionResultViewModel> GameWinJackpotAsync(
        [FromServices] IGetService<GameWinJackpotCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameWinJackpotViewModel request)
    {
        var result = await service
            .GetAsync(new GameWinJackpotCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }
}
