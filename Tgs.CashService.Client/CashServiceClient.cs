using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.CashService.Client;

internal class CashServiceClient : ICashServiceClient
{
    private static readonly JsonSerializerOptions _SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private readonly HttpClient _httpClient;

    public CashServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async ValueTask<TransactionResponse> AdjustCompanyAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId)
    {
        var request = new
        {
            Amount = amount,
            Description = description,
            ExternalId = externalId,
            CreatedByUserId = createdByUserId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/adjust/company/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<decimal> GetBalanceAsync(Guid walletId)
    {
        var response = await _httpClient
            .GetAsync($"/wallet/{walletId}")
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<decimal>(contentJson);
    }

    public async ValueTask<TransactionResponse> BonusAsync(Guid walletId, decimal amount, string? description,
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
                $"/wallet/bonus/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> CancelWithdrawalAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createdByUserId)
    {
        var request = new
        {
            Amount = amount,
            Description = description,
            ExternalId = externalId,
            CreatedByUserId = createdByUserId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        using var requestMessage = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/wallet/withdrawal/{walletId}")
        {
            Content = requestContent
        };
        var response = await _httpClient
            .SendAsync(requestMessage)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> DepositAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createdByUserId)
    {
        var request = new
        {
            Amount = amount,
            Description = description,
            ExternalId = externalId,
            CreatedByUserId = createdByUserId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/deposit/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderDepositAsync(Guid walletId, decimal amount,
        string? description, string? externalId)
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
                $"/wallet/gp/deposit/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderFundInAsync(Guid walletId, decimal amount,
        string? description, string? externalId)
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
                $"/wallet/gp/fund-in/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderFundOutAsync(Guid walletId, decimal amount,
        string? description, string? externalId)
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
                $"/wallet/gp/fund-out/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderWithdrawalAsync(Guid walletId, decimal amount,
        string? description, string? externalId)
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
                $"/wallet/gp/withdrawal/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> OtherAsync(
        Guid walletId,
        decimal amount,
        string description,
        string externalId,
        Guid createdByUserId)
    {
        var request = new
        {
            Amount = amount,
            Description = description,
            ExternalId = externalId,
            CreatedByUserId = createdByUserId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/other/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> PromotionAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createByUserId)

    {
        var request = new
        {
            Amount = amount,
            Description = description,
            ExternalId = externalId,
            CreateByUserId = createByUserId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/promotion/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> RebateAsync(Guid walletId, decimal amount, string? description,
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
                $"/wallet/rebate/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> WithdrawalAsync(Guid walletId, decimal amount, string? description,
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
                $"/wallet/withdrawal/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> UnsettledBetManualAsync(
        Guid walletId,
        decimal amount,
        string description,
        string externalId,
        Guid createdByUserId)
    {
        var request = new
        {
            Amount = amount,
            Description = description,
            ExternalId = externalId,
            CreatedByUserId = createdByUserId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/unsettled/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> AdjustResettledBetManualAsync(
        Guid walletId,
        decimal amount,
        string description,
        string externalId,
        Guid createdByUserId)
    {
        var request = new
        {
            Amount = amount,
            Description = description,
            ExternalId = externalId,
            CreatedByUserId = createdByUserId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/adjust/resettled/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderPlaceBetAsync(Guid walletId, decimal amount, string externalId)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/gp/bet/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderSettleWinAsync(Guid walletId, decimal amount, string externalId)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/gp/settle-win/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> DividendAllAgentRebateAsync(Guid walletId, decimal amount,
        string? description, string? externalId)
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
                $"/wallet/dividend/allAgentRebate/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> PromotionVipLevelUpgradeAsync(Guid walletId, decimal amount,
        string? description, string? externalId)
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
                $"/wallet/promotion/vip/upgrad/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> PromotionVipLevelSeasonAsync(Guid walletId, decimal amount,
        string? description, string? externalId)
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
                $"/wallet/promotion/vip/season/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> AdjustAgentAsync(Guid walletId, decimal amount, string? description,
        string? externalId, Guid createdByUserId)
    {
        var request = new
        {
            Amount = amount,
            Description = description,
            ExternalId = externalId,
            CreatedByUserId = createdByUserId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/adjust/agent/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> AdjustSystemAsync(Guid walletId, decimal amount, string? description,
        string? externalId, Guid createdByUserId)
    {
        var request = new
        {
            Amount = amount,
            Description = description,
            ExternalId = externalId,
            CreatedByUserId = createdByUserId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/adjust/system/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> AdjustTransferAsync(Guid walletId, decimal amount, string? description,
        string? externalId, Guid createdByUserId)
    {
        var request = new
        {
            Amount = amount,
            Description = description,
            ExternalId = externalId,
            CreatedByUserId = createdByUserId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/adjust/transfer/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderSettleLoseAsync(Guid walletId, decimal amount, string externalId)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/gp/settle-lose/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderSettleTieAsync(Guid walletId, decimal amount, string externalId)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/gp/settle-tie/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderSettleFreeAsync(Guid walletId, decimal amount, string externalId)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/gp/settle-free/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderCancelPlaceBetAsync(Guid walletId, decimal amount, string externalId)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/gp/cancel-bet/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderCancelSettlementAsync(Guid walletId, decimal amount, string externalId)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/gp/cancel-settle/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderLockAmountAsync(Guid walletId, decimal amount, string externalId)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/gp/lock/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderUnlockAmountAsync(Guid walletId, decimal amount, string externalId)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync(
                $"/wallet/gp/unlock/{walletId}",
                requestContent)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }
}
