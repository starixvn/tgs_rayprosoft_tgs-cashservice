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
    [HttpPut("game/bet/{walletId}", Name = "GameBet")]
    public async ValueTask<TransactionResultViewModel> GameBetAsync(
        [FromServices] IGetService<GameBetCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameBetViewModel request)
    {
        var result = await service
            .GetAsync(new GameBetCommand(
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
    [HttpPut("game/lock/{walletId}", Name = "GameLock")]
    public async ValueTask<TransactionResultViewModel> GameLockAsync(
        [FromServices] IGetService<GameLockCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameLockViewModel request)
    {
        var result = await service
            .GetAsync(new GameLockCommand(
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
    [HttpPut("game/unlock/{walletId}", Name = "GameUnlock")]
    public async ValueTask<TransactionResultViewModel> GameUnlockAsync(
        [FromServices] IGetService<GameUnlockCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameUnlockViewModel request)
    {
        var result = await service
            .GetAsync(new GameUnlockCommand(
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
    [HttpPut("game/settle-lose/{walletId}", Name = "GameSettleLose")]
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
    [HttpPut("game/cancel/{walletId}", Name = "GameCancel")]
    public async ValueTask<TransactionResultViewModel> GameCancelAsync(
        [FromServices] IGetService<GameCancelCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameCancelViewModel request)
    {
        var result = await service
            .GetAsync(new GameCancelCommand(
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

    /// <summary>
    /// Games the credit asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/credit/{walletId}", Name = "GameCredit")]
    public async ValueTask<TransactionResultViewModel> GameCreditAsync(
        [FromServices] IGetService<GameCreditCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameCreditViewModel request)
    {
        var result = await service
            .GetAsync(new GameCreditCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    /// Games the debit asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/debit/{walletId}", Name = "GameDebit")]
    public async ValueTask<TransactionResultViewModel> GameDebitAsync(
        [FromServices] IGetService<GameDebitCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameDebitViewModel request)
    {
        var result = await service
            .GetAsync(new GameDebitCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    /// Games the refund asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/refund/{walletId}", Name = "GameRefund")]
    public async ValueTask<TransactionResultViewModel> GameRefundAsync(
        [FromServices] IGetService<GameRefundCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameRefundViewModel request)
    {
        var result = await service
            .GetAsync(new GameRefundCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }

    /// <summary>
    /// Games the revision asynchronous.
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
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("game/revision/{walletId}", Name = "GameRevision")]
    public async ValueTask<TransactionResultViewModel> GameRevisionAsync(
        [FromServices] IGetService<GameRevisionCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameRevisionCommand request)
    {
        var result = await service
            .GetAsync(new GameRevisionCommand(
                walletId,
                request.Amount,
                request.ExternalId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance, result.LockedBalance);
    }
}
