using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers;

[Route("[controller]")]
[ApiController]
public class WalletController : ControllerBase
{
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
    [HttpPut("adjust/company/{walletId}", Name = "AdjustCompany")]
    public async ValueTask<TransactionResultViewModel> AdjustCompanyAsync(
        [FromServices] IGetService<AdjustCompanyCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] AdjustCompanyViewModel request)
    {
        var result = await service
            .GetAsync(new AdjustCompanyCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId,
                request.CreatedByUserId), this.CreateCancellationToken())
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
                request.ExternalId), this.CreateCancellationToken())
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
                request.CreatedByUserId), this.CreateCancellationToken())
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
                request.CreatedByUserId), this.CreateCancellationToken())
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
                request.CreatedByUserId), this.CreateCancellationToken())
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
                request.CreatedByUserId), this.CreateCancellationToken())
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
                request.ExternalId), this.CreateCancellationToken())
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
                request.ExternalId), this.CreateCancellationToken())
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
    ///     "description": "測試VIPLevel生日福利",
    ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    [HttpPut("promotion/vip/birthday/{walletId}", Name = "PromotionVipLevelBirthday")]
    public async ValueTask<TransactionResultViewModel> PromotionVipLevelBirthdayAsync(
        [FromServices] IGetService<PromotionVipLevelBirthdayCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] PromotionVipLevelSeasonViewModel request)
    {
        var result = await service
            .GetAsync(new PromotionVipLevelBirthdayCommand(
                walletId,
                request.Amount,
                request.Description,
                request.ExternalId), this.CreateCancellationToken())
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
                viewModel.ExternalId), this.CreateCancellationToken())
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
                viewModel.ExternalId), this.CreateCancellationToken())
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
    [HttpPut("adjust/agent/{walletId}", Name = "AdjustAgent")]
    public async ValueTask<TransactionResultViewModel> AdjustAgentAsync(
        [FromServices] IGetService<AdjustAgentCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] AdjustAgentViewModel viewModel)
    {
        var result = await service
            .GetAsync(new AdjustAgentCommand(
                walletId,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId,
                viewModel.CreatedByUserId), this.CreateCancellationToken())
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
    [HttpPut("adjust/system/{walletId}", Name = "AdjustSystem")]
    public async ValueTask<TransactionResultViewModel> AdjustSystemAsync(
        [FromServices] IGetService<AdjustSystemCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] AdjustSystemViewModel viewModel)
    {
        var result = await service
            .GetAsync(new AdjustSystemCommand(
                walletId,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId,
                viewModel.CreatedByUserId), this.CreateCancellationToken())
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
    [HttpPut("adjust/transfer/{walletId}", Name = "AdjustTransfer")]
    public async ValueTask<TransactionResultViewModel> AdjustTransferAsync(
        [FromServices] IGetService<AdjustTransferCommand, ExchangeResult> service,
        Guid walletId,
        [FromBody] AdjustTransferViewModel viewModel)
    {
        var result = await service
            .GetAsync(new AdjustTransferCommand(
                walletId,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId,
                viewModel.CreatedByUserId), this.CreateCancellationToken())
            .ConfigureAwait(false);

        return new TransactionResultViewModel(result.TransactionId, result.Balance);
    }
}
