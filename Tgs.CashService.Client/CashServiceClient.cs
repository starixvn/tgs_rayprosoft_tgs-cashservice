using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tgs.CashService.Client
{
    internal class CashServiceClient : ICashServiceClient
    {
        private static readonly JsonSerializerOptions _SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {
                new JsonStringEnumConverter()
            }
        };

        private readonly HttpClient _httpClient;

        public CashServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async ValueTask<TransactionResponse?> AdjustAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId, Guid createdByUserId)
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Adjust",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<decimal> BalanceAsync(Guid walletId)
        {
            var response = await _httpClient.GetAsync("/Wallet/Balance/{walletId}")
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<decimal>(contentJson);
        }

        public async ValueTask<TransactionResponse?> BonusAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Bonus",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<TransactionResponse?> CancelWithdrawalAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId, Guid createdByUserId)
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/CancelWithdrawal",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<TransactionResponse> DepositAsync(
            Guid walletId,
            decimal amount,
            string? description,
            string? externalId)
        {
            var request = new
            {
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"/wallet/deposit/{walletId}",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
        }

        public async ValueTask<TransactionResponse?> DividendAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Dividend",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<TransactionResponse?> GameProviderDepositAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/GameProviderDeposit",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<TransactionResponse?> GameProviderFundInAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/GameProviderFundIn",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<TransactionResponse?> GameProviderFundOutAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/GameProviderFundOut",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<TransactionResponse?> GameProviderWithdrawalAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/GameProviderWithdrawal",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<TransactionResponse?> OtherAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId, Guid createdByUserId)
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Other",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<TransactionResponse?> PromotionAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)

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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Promotion",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<TransactionResponse?> RebateAsync(Guid brandId, Guid walletId, string playerCode, decimal amount, string? description, string? externalId)
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   "/Wallet/Rebate",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<TransactionResponse> WithdrawalAsync(Guid walletId, decimal amount, string? description, string? externalId)
        {
            var request = new
            {
                Amount = amount,
                Description = description,
                ExternalId = externalId
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PutAsync(
                   $"/wallet/withdrawal/{walletId}",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
        }
    }
}
