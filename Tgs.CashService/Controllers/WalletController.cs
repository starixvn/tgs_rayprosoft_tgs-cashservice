using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Core;
using Tgs.CashService.Core.ApplicationServices;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ILogger<WalletController> _logger;

        public WalletController(ILogger<WalletController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Adjusts the exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試調整",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///             "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
        ///         }
        /// </remarks>
        [HttpPost("Adjust")]
        public async ValueTask<TransactionViewModel> AdjustAsync(
                [FromServices] IGetService<AdjustQuery, ExchangeResult> service,
                [FromBody] AdjustViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId,
                CreatedByUserId = viewModel.CreatedByUserId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Balances the asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="walletId">The wallet identifier.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Get
        ///         api/[controller]/36a6cdef-094f-eb11-8d88-70b5e82b6b8d
        /// </remarks>
        [HttpGet("{walletId}")]
        public async ValueTask<decimal> BalanceAsync(
                [FromServices] IGetService<BalanceQuery, decimal?> service,
                Guid walletId)
        {
            var result = await service.GetAsync(new()
            {
                WalletId = walletId
            }).ConfigureAwait(false);
            return result.Value;
        }

        /// <summary>
        /// Bonuses the exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試紅利",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///         }
        /// </remarks>
        [HttpPost("Bonus")]
        public async ValueTask<TransactionViewModel> BonusAsync(
                [FromServices] IGetService<BonusQuery, ExchangeResult> service,
                [FromBody] BonusViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Cancels the withdrawal exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試取消提款",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///             "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
        ///         }
        /// </remarks>
        [HttpPost("CancelWithdrawal")]
        public async ValueTask<TransactionViewModel> CancelWithdrawalAsync(
                [FromServices] IGetService<CancelWithdrawalQuery, ExchangeResult> service,
                [FromBody] CancelWithdrawalViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId,
                CreatedByUserId = viewModel.CreatedByUserId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Deposits the exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試存款",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///         }
        /// </remarks>
        [HttpPost("Deposit")]
        public async ValueTask<TransactionViewModel> DepositAsync(
                [FromServices] IGetService<DepositQuery, ExchangeResult> service,
                [FromBody] DepositViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Dividends the exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試分紅",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///         }
        /// </remarks>
        [HttpPost("Dividend")]
        public async ValueTask<TransactionViewModel> DividendAsync(
                [FromServices] IGetService<DividendQuery, ExchangeResult> service,
                [FromBody] DividendViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Games the provider deposit exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試遊戲上分",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///         }
        /// </remarks>
        [HttpPost("GameProviderDeposit")]
        public async ValueTask<TransactionViewModel> GameProviderDepositAsync(
                [FromServices] IGetService<GameProviderDepositQuery, ExchangeResult> service,
                [FromBody] GameProviderDepositViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Games the provider fund in exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試主錢包轉入(GP->TGS)",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///         }
        /// </remarks>
        [HttpPost("GameProviderFundIn")]
        public async ValueTask<TransactionViewModel> GameProviderFundInAsync(
                [FromServices] IGetService<GameProviderFundInQuery, ExchangeResult> service,
                [FromBody] GameProviderFundInViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Games the provider fund out exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試主錢包轉出(TGS->GP)",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///         }
        /// </remarks>
        [HttpPost("GameProviderFundOut")]
        public async ValueTask<TransactionViewModel> GameProviderFundOutAsync(
                [FromServices] IGetService<GameProviderFundOutQuery, ExchangeResult> service,
                [FromBody] GameProviderFundOutViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Games the provider withdrawal exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試遊戲下分",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///         }
        /// </remarks>
        [HttpPost("GameProviderWithdrawal")]
        public async ValueTask<TransactionViewModel> GameProviderWithdrawalAsync(
                [FromServices] IGetService<GameProviderWithdrawalQuery, ExchangeResult> service,
                [FromBody] GameProviderWithdrawalViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Others the exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試其他",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///             "createdByUserId": "f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"
        ///         }
        /// </remarks>
        [HttpPost("Other")]
        public async ValueTask<TransactionViewModel> OtherAsync(
                [FromServices] IGetService<OtherQuery, ExchangeResult> service,
                [FromBody] OtherViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId,
                CreatedByUserId = viewModel.CreatedByUserId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Promotions the exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試活動",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///         }
        /// </remarks>
        [HttpPost("Promotion")]
        public async ValueTask<TransactionViewModel> PromotionAsync(
                [FromServices] IGetService<PromotionQuery, ExchangeResult> service,
                [FromBody] PromotionViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Rebates the exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試活動",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///         }
        /// </remarks>
        [HttpPost("Rebate")]
        public async ValueTask<TransactionViewModel> RebateAsync(
                [FromServices] IGetService<RebateQuery, ExchangeResult> service,
                [FromBody] RebateViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }

        /// <summary>
        /// Withdrawals the exchange asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
        ///             "playerCode": "4ucmqplxb029",
        ///             "amount": 10,
        ///             "description": "測試提款",
        ///             "externalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///         }
        /// </remarks>
        [HttpPost("Withdrawal")]
        public async ValueTask<TransactionViewModel> WithdrawalAsync(
                [FromServices] IGetService<WithdrawalQuery, ExchangeResult> service,
                [FromBody] WithdrawalViewModel viewModel)
        {
            var result = await service.GetAsync(new()
            {
                BrandId = viewModel.BrandId,
                WalletId = viewModel.WalletId,
                PlayerCode = viewModel.PlayerCode,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                ExternalId = viewModel.ExternalId
            })
                .ConfigureAwait(false);

            return new()
            {
                TransactionId = result.TransactionId,
                Balance = result.Balance
            };
        }
    }
}
