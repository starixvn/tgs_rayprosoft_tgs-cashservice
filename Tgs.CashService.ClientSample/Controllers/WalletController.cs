using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.CashService.ClientSample.Models;

namespace Tgs.CashService.ClientSample.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ICashServiceClient _cashServiceClinet;
        private readonly ILogger<WalletController> _logger;

        public WalletController(
            ILogger<WalletController> logger,
            ICashServiceClient cashServiceClinet)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cashServiceClinet = cashServiceClinet ?? throw new ArgumentNullException(nameof(cashServiceClinet));
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
        public ValueTask<TransactionResponse?> AdjustAsync(
                [FromBody] AdjustViewModel viewModel)
            => _cashServiceClinet.AdjustAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId,
                viewModel.CreatedByUserId);

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
        public ValueTask<decimal> BalanceAsync(
                Guid walletId)
            => _cashServiceClinet.BalanceAsync(walletId);

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
        public ValueTask<TransactionResponse?> BonusAsync(
            [FromBody] BonusViewModel viewModel)
            => _cashServiceClinet.BonusAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId);

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
        public ValueTask<TransactionResponse?> CancelWithdrawalAsync(
                [FromBody] CancelWithdrawalViewModel viewModel)
            => _cashServiceClinet.CancelWithdrawalAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId,
                viewModel.CreatedByUserId);

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
        public ValueTask<TransactionResponse?> DepositAsync(
            [FromBody] DepositViewModel viewModel)
            => _cashServiceClinet.DepositAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId);

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
        public ValueTask<TransactionResponse?> DividendAsync(
            [FromBody] DividendViewModel viewModel)
            => _cashServiceClinet.DividendAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId);

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
        public ValueTask<TransactionResponse?> GameProviderDepositAsync(
            [FromBody] GameProviderDepositViewModel viewModel)
            => _cashServiceClinet.GameProviderDepositAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId);

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
        public ValueTask<TransactionResponse?> GameProviderFundInAsync(
                [FromBody] GameProviderFundInViewModel viewModel)
            => _cashServiceClinet.GameProviderFundInAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId);

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
        public ValueTask<TransactionResponse?> GameProviderFundOutAsync(
                [FromBody] GameProviderFundOutViewModel viewModel)
            => _cashServiceClinet.GameProviderFundOutAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId);

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
        public ValueTask<TransactionResponse?> GameProviderWithdrawalAsync(
            [FromBody] GameProviderWithdrawalViewModel viewModel)
            => _cashServiceClinet.GameProviderWithdrawalAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId);

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
        public ValueTask<TransactionResponse?> OtherAsync(
            [FromBody] OtherViewModel viewModel)
            => _cashServiceClinet.OtherAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId,
                viewModel.CreatedByUserId);

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
        public ValueTask<TransactionResponse?> PromotionAsync(
                [FromBody] PromotionViewModel viewModel)
            => _cashServiceClinet.PromotionAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId);

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
        public ValueTask<TransactionResponse?> RebateAsync(
                [FromBody] RebateViewModel viewModel)
            => _cashServiceClinet.RebateAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId);

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
        public ValueTask<TransactionResponse?> WithdrawalAsync(
            [FromBody] WithdrawalViewModel viewModel)
            => _cashServiceClinet.WithdrawalAsync(
                viewModel.BrandId,
                viewModel.WalletId,
                viewModel.PlayerCode,
                viewModel.Amount,
                viewModel.Description,
                viewModel.ExternalId);
    }
}
