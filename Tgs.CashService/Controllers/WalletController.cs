using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.Core;
using Tgs.CashService.Core.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers
{
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試調整",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///     "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
        ///     }
        /// </remarks>
        [HttpPut("adjust/{walletId}")]
        public async ValueTask<TransactionResultViewModel> AdjustAsync(
            [FromServices] IGetService<AdjustCommand, ExchangeResult> service,
            Guid walletId,
            [FromBody] AdjustViewModel request)
        {
            var result = await service
                .GetAsync(new AdjustCommand(
                    walletId,
                    request.Amount,
                    request.Description,
                    request.ExternalId,
                    request.CreatedByUserId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        ///     Balances the asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="walletId">The wallet identifier.</param>
        /// <returns></returns>
        /// <remarks>
        ///     <b>Example</b><br />
        ///     Get /36a6cdef-094f-eb11-8d88-70b5e82b6b8d
        /// </remarks>
        [HttpGet("{walletId}")]
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試紅利",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        /// </remarks>
        [HttpPut("bonus/{walletId}")]
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
                    request.ExternalId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試取消提款",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///     "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
        ///     }
        /// </remarks>
        [HttpDelete("withdrawal/{walletId}")]
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
                    request.CreatedByUserId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        ///     Deposits the exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="walletId"></param>
        /// <param name="request">The view model.</param>
        /// <returns></returns>
        [HttpPut("deposit/{walletId}")]
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
                    request.CreatedByUserId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試遊戲上分",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        /// </remarks>
        [HttpPut("gp/deposit/{walletId}")]
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
                    request.ExternalId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試主錢包轉入(GP->TGS)",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        /// </remarks>
        [HttpPut("gp/fund-in/{walletId}")]
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
                    viewModel.ExternalId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試主錢包轉出(TGS->GP)",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        /// </remarks>
        [HttpPut("gp/fund-out/{walletId}")]
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
                    request.ExternalId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試遊戲下分",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        /// </remarks>
        [HttpPut("gp/withdrawal/{walletId}")]
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
                    request.ExternalId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="walletId"></param>
        /// <param name="request">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        ///     <b>Example</b><br />
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試Open Round Page",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///     "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
        ///     }
        /// </remarks>
        [HttpPut("unsettled/{walletId}")]
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
                    request.CreatedByUserId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>        
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="walletId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        ///     <b>Example</b><br />
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試Closed Round Page",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///     "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
        ///     }
        /// </remarks>
        [HttpPut("resettled/{walletId}")]
        public async ValueTask<TransactionResultViewModel> ResettledBetManualAsync(
            [FromServices] IGetService<ResettledBetManualCommand, ExchangeResult> service,
            Guid walletId,
            [FromBody] ResettledBetManualViewModel request)
        {
            var result = await service
                .GetAsync(new ResettledBetManualCommand(
                    walletId,
                    request.Amount,
                    request.Description,
                    request.ExternalId,
                    request.CreatedByUserId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試其他",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///     "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
        ///     }
        /// </remarks>
        [HttpPut("other/{walletId}")]
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
                    request.CreatedByUserId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試活動",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        /// </remarks>
        [HttpPut("promotion/{walletId}")]
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
                    request.CreatedByUserId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試活動",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        /// </remarks>
        [HttpPut("rebate/{walletId}")]
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
                    viewModel.ExternalId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試提款",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        /// </remarks>
        [HttpPut("withdrawal/{walletId}")]
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
                    request.ExternalId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
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
        ///     Post
        ///     {
        ///     "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///     "amount": 10,
        ///     "description": "測試活動",
        ///     "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        /// </remarks>
        [HttpPut("allAgentRebate/{walletId}")]
        public async ValueTask<TransactionResultViewModel> AllAgentRebateAsync(
            [FromServices] IGetService<AllAgentRebateCommand, ExchangeResult> service,
            Guid walletId,
            [FromBody] RebateViewModel viewModel)
        {
            var result = await service
                .GetAsync(new AllAgentRebateCommand(
                    walletId,
                    viewModel.Amount,
                    viewModel.Description,
                    viewModel.ExternalId))
                .ConfigureAwait(false);

            return new TransactionResultViewModel
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }
    }
}