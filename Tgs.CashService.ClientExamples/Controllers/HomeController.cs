using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.Client;

namespace Tgs.CashService.ClientExamples.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICashServiceClient _cashServiceClient;

        public HomeController(ILogger<HomeController> logger, ICashServiceClient cashServiceClient)
        {
            _logger = logger;
            _cashServiceClient = cashServiceClient;
        }

        public async Task<IActionResult> Index()
        {
            var walletId = new Guid("12c06da5-5487-4b5f-b73a-e5fdf2b2eada");
            var playerCode = "4unwqobrfsro";
            var requestDescription = "In the real integration, this description contains all metadata of the request for tracing issue";

            // Get Balance
            var walletAmount = await _cashServiceClient.GetBalanceAsync(walletId);

            // Deduct Balance
            var deductAmount = 10m;
            var deductTransaction = await _cashServiceClient.GameProviderDeductBalanceAsync(playerCode, deductAmount, requestDescription);

            // Add Balance
            var addAmount = 9m;
            var addTransaction = await _cashServiceClient.GameProviderAddBalanceAsync(playerCode, addAmount, requestDescription);


            return Content(walletAmount.ToString());
        }
    }
}
