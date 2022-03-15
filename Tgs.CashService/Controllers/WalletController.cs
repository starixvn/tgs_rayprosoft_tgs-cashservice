using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers;

[Route("[controller]")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly ILogger<WalletController> _logger;

    public WalletController(ILogger<WalletController> logger)
        => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Adjusts the exchange asynchronous.
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
    ///     "description": "測試公司調帳",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
    ///     }
    /// </remarks>
    [HttpPut("adjust/company/{walletId}", Name = "CompanyAdjust")]
    public async ValueTask<TransactionResultViewModel> CompanyAdjustAsync(
        [FromServices] IGetService<CompanyAdjustCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] CompanyAdjustViewModel request)
    {
        var result = await service
            .GetAsync(new CompanyAdjustCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId,
                request.CreatedByUserId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Balances the asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="walletId">The wallet identifier.</param>
    /// <returns></returns>
    /// <remarks>
    ///     <b>Example</b><br />
    ///     Get
    ///     /36a6cdef-094f-eb11-8d88-70b5e82b6b8d
    /// </remarks>
    [HttpGet("{walletId}", Name = "GetBalance")]
    public ValueTask<decimal> GetBalanceAsync(
        [FromServices] IGetService<GetBalanceQuery, decimal> service,
        Guid walletId)
        => service.GetAsync(new GetBalanceQuery(walletId));

    /// <summary>
    ///     Bonuses the exchange asynchronous.
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
    ///     "description": "測試紅利",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("bonus/{walletId}", Name = "Bonus")]
    public async ValueTask<TransactionResultViewModel> BonusAsync(
        [FromServices] IGetService<BonusCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] BonusViewModel request)
    {
        var result = await service
            .GetAsync(new BonusCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Cancels the withdrawal exchange asynchronous.
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
                request.CreatedByUserId), CreateCancellationToken())
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
                request.CreatedByUserId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

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
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
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
                viewModel.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
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
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
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
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Games the provider place bet exchange asynchronous.
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
    ///     "description": "測試遊戲下注",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("gp/bet/{walletId}", Name = "GameProviderPlaceBet")]
    public async ValueTask<TransactionResultViewModel> GameProviderPlaceBetAsync(
        [FromServices] IGetService<GameProviderPlaceBetCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameProviderPlaceBetViewModel request)
    {
        var result = await service
            .GetAsync(new GameProviderPlaceBetCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Games the provider settle win bet exchange asynchronous.
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
    ///     "description": "測試遊戲結算贏",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("gp/settle-win/{walletId}", Name = "GameProviderSettleWin")]
    public async ValueTask<TransactionResultViewModel> GameProviderSettleWinAsync(
        [FromServices] IGetService<GameProviderSettleWinCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameProviderSettleWinViewModel request)
    {
        var result = await service
            .GetAsync(new GameProviderSettleWinCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Games the provider settle lose bet exchange asynchronous.
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
    ///     "description": "測試遊戲結算輸",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("gp/settle-lose/{walletId}", Name = "GameProviderSettleLose")]
    public async ValueTask<TransactionResultViewModel> GameProviderSettleLoseAsync(
        [FromServices] IGetService<GameProviderSettleLoseCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameProviderSettleLoseViewModel request)
    {
        var result = await service
            .GetAsync(new GameProviderSettleLoseCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Games the provider settle tie bet exchange asynchronous.
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
    ///     "description": "測試遊戲結算和",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("gp/settle-tie/{walletId}", Name = "GameProviderSettleTie")]
    public async ValueTask<TransactionResultViewModel> GameProviderSettleTieAsync(
        [FromServices] IGetService<GameProviderSettleTieCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameProviderSettleTieViewModel request)
    {
        var result = await service
            .GetAsync(new GameProviderSettleTieCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Games the provider settle free bet exchange asynchronous.
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
    ///     "description": "測試遊戲結算免費投注",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("gp/settle-free/{walletId}", Name = "GameProviderSettleFree")]
    public async ValueTask<TransactionResultViewModel> GameProviderSettleFreeAsync(
        [FromServices] IGetService<GameProviderSettleFreeCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] GameProviderSettleFreeViewModel request)
    {
        var result = await service
            .GetAsync(new GameProviderSettleFreeCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Unsettleds the bet manual asynchronous.
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
    ///     "description": "測試Open Round Page",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
    ///     }
    /// </remarks>
    [HttpPut("unsettled/{walletId}", Name = "UnsettledBetManual")]
    public async ValueTask<TransactionResultViewModel> UnsettledBetManualAsync(
        [FromServices] IGetService<UnsettledBetManualCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] UnsettledBetManualViewModel request)
    {
        var result = await service
            .GetAsync(new UnsettledBetManualCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId,
                request.CreatedByUserId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Adjust the exchange for the cloesd game round
    ///     Resettleds the bet manual asynchronous.
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
    ///     "description": "測試Closed Round Page",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
    ///     }
    /// </remarks>
    [HttpPut("adjust/resettled/{walletId}", Name = "AdjustResettledBetManual")]
    public async ValueTask<TransactionResultViewModel> AdjustResettledBetManualAsync(
        [FromServices] IGetService<AdjustResettledBetManualCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] AdjustResettledBetManualViewModel request)
    {
        var result = await service
            .GetAsync(new AdjustResettledBetManualCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId,
                request.CreatedByUserId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Others the exchange asynchronous.
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
    ///     "description": "測試其他",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
    ///     }
    /// </remarks>
    [HttpPut("other/{walletId}", Name = "Other")]
    public async ValueTask<TransactionResultViewModel> OtherAsync(
        [FromServices] IGetService<OtherCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] OtherViewModel request)
    {
        var result = await service
            .GetAsync(new OtherCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId,
                request.CreatedByUserId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Promotions the exchange asynchronous.
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
    ///     "description": "測試活動",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("promotion/{walletId}", Name = "Promotion")]
    public async ValueTask<TransactionResultViewModel> PromotionAsync(
        [FromServices] IGetService<PromotionCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] PromotionViewModel request)
    {
        var result = await service
            .GetAsync(new PromotionCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId,
                request.CreatedByUserId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Promotions the vip level season asynchronous.
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
    ///     "description": "測試VIPLevel季福利",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("promotion/vip/season/{walletId}", Name = "PromotionVipLevelSeason")]
    public async ValueTask<TransactionResultViewModel> PromotionVipLevelSeasonAsync(
        [FromServices] IGetService<PromotionVipLevelSeasonCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] PromotionVipLevelSeasonViewModel request)
    {
        var result = await service
            .GetAsync(new PromotionVipLevelSeasonCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Promotions the vip level upgrade asynchronous.
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
    ///     "description": "測試VIPLevel升級福利",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("promotion/vip/upgrad/{walletId}", Name = "PromotionVipLevelUpgrad")]
    public async ValueTask<TransactionResultViewModel> PromotionVipLevelUpgradeAsync(
        [FromServices] IGetService<PromotionVipLevelUpgradeCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] PromotionVipLevelUpgradViewModel request)
    {
        var result = await service
            .GetAsync(new PromotionVipLevelUpgradeCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Rebates the exchange asynchronous.
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
    ///     "description": "測試活動",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("rebate/{walletId}", Name = "Rebate")]
    public async ValueTask<TransactionResultViewModel> RebateAsync(
        [FromServices] IGetService<RebateCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] RebateViewModel viewModel)
    {
        var result = await service
            .GetAsync(new RebateCommand(
                walletId,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Withdrawals the exchange asynchronous.
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
                request.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Dividends all agent rebate asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="walletId">The wallet identifier.</param>
    /// <param name="viewModel">The view model.</param>
    /// <returns></returns>
    /// <remarks>
    ///     <b>Example</b><br />
    ///     Post WalletId: 36a6cdef-094f-eb11-8d88-70b5e82b6b8d
    ///     {
    ///     "amount": 10,
    ///     "description": "測試PlaceBet",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("dividend/allAgentRebate/{walletId}", Name = "DividendAllAgentRebate")]
    public async ValueTask<TransactionResultViewModel> DividendAllAgentRebateAsync(
        [FromServices] IGetService<DividendAllAgentRebateCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] RebateViewModel viewModel)
    {
        var result = await service
            .GetAsync(new DividendAllAgentRebateCommand(
                walletId,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Agent adjust the account asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="walletId">The wallet identifier.</param>
    /// <param name="viewModel">The view model.</param>
    /// <returns></returns>
    /// <remarks>
    ///     <b>Example</b><br />
    ///     Post WalletId: 36a6cdef-094f-eb11-8d88-70b5e82b6b8d
    ///     {
    ///     "amount": 10,
    ///     "description": "測試代理調帳",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("adjust/agent/{walletId}", Name = "AgentAdjust")]
    public async ValueTask<TransactionResultViewModel> AgentAdjustAsync(
        [FromServices] IGetService<AgentAdjustCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] AgentAdjustViewModel viewModel)
    {
        var result = await service
            .GetAsync(new AgentAdjustCommand(
                walletId,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId,
                viewModel.CreatedByUserId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     System adjust the account asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="walletId">The wallet identifier.</param>
    /// <param name="viewModel">The view model.</param>
    /// <returns></returns>
    /// <remarks>
    ///     <b>Example</b><br />
    ///     Post WalletId: 36a6cdef-094f-eb11-8d88-70b5e82b6b8d
    ///     {
    ///     "amount": 10,
    ///     "description": "測試系統調帳",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("adjust/system/{walletId}", Name = "SystemAdjust")]
    public async ValueTask<TransactionResultViewModel> SystemAdjustAsync(
        [FromServices] IGetService<SystemAdjustCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] SystemAdjustViewModel viewModel)
    {
        var result = await service
            .GetAsync(new SystemAdjustCommand(
                walletId,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId,
                viewModel.CreatedByUserId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    /// <summary>
    ///     Transfer adjust the account asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="walletId">The wallet identifier.</param>
    /// <param name="viewModel">The view model.</param>
    /// <returns></returns>
    /// <remarks>
    ///     <b>Example</b><br />
    ///     Post WalletId: 36a6cdef-094f-eb11-8d88-70b5e82b6b8d
    ///     {
    ///     "amount": 10,
    ///     "description": "測試系統轉帳",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("adjust/transfer/{walletId}", Name = "TransferAdjust")]
    public async ValueTask<TransactionResultViewModel> TransferAdjustAsync(
        [FromServices] IGetService<TransferAdjustCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] TransferAdjustViewModel viewModel)
    {
        var result = await service
            .GetAsync(new TransferAdjustCommand(
                walletId,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId,
                viewModel.CreatedByUserId), CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }

    private CancellationToken CreateCancellationToken()
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(HttpContext.RequestAborted);
        cts.CancelAfter(TimeSpan.FromSeconds(10));

        return cts.Token;
    }
}
