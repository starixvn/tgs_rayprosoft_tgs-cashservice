using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tgs.CashService.Client
{
    internal class CashServiceClient : ICashServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions? _serializerOptions;

        public CashServiceClient(
            HttpClient httpClient,
            JsonSerializerOptions? serializerOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializerOptions = serializerOptions;
        }

        public async ValueTask<TransactionViewModel?> AdjustAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId, Guid createdByUserId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId,
                CreatedByUserId = createdByUserId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Adjust",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<decimal> BalanceAsync(Guid walletId)
        {
            var response = await _httpClient.GetAsync("/Wallet/Balance/{walletId}")
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<decimal>(contentJson);
        }

        public async ValueTask<TransactionViewModel?> BonusAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Bonus",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<TransactionViewModel?> CancelWithdrawalAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId, Guid createdByUserId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId,
                CreatedByUserId = createdByUserId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/CancelWithdrawal",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<TransactionViewModel?> DepositAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Deposit",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<TransactionViewModel?> DividendAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Dividend",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<TransactionViewModel?> GameProviderDepositAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/GameProviderDeposit",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<TransactionViewModel?> GameProviderFundInAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/GameProviderFundIn",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<TransactionViewModel?> GameProviderFundOutAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/GameProviderFundOut",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<TransactionViewModel?> GameProviderWithdrawalAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/GameProviderWithdrawal",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<TransactionViewModel?> OtherAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId, Guid createdByUserId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId,
                CreatedByUserId = createdByUserId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Other",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<TransactionViewModel?> PromotionAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)

        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Promotion",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<TransactionViewModel?> RebateAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Rebate",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }

        public async ValueTask<TransactionViewModel?> WithdrawalAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
        {
            var request = new
            {
                BrandId = brandId,
                WalletId = walletId,
                PlayerCode = playerCode,
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Withdrawal",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionViewModel>(contentJson, _serializerOptions);
        }
    }
}
