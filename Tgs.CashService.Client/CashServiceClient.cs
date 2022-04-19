using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.CashService.Client;

internal partial class CashServiceClient : ICashServiceClient
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
        Guid createdByUserId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/adjust/company/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<decimal> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
            .GetAsync($"/wallet/{walletId}", cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<decimal>(contentJson);
    }

    public async ValueTask<TransactionResponse> BonusAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/bonus/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderDepositAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/gp/deposit/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderFundInAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/gp/fund-in/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderFundOutAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/gp/fund-out/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderWithdrawalAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/gp/withdrawal/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> OtherAsync(
        Guid walletId,
        decimal amount,
        string description,
        string externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/other/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> PromotionAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createByUserId,
        CancellationToken cancellationToken = default)

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
            .PutAsync($"/wallet/promotion/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> RebateAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/rebate/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> UnsettledBetManualAsync(
        Guid walletId,
        decimal amount,
        string description,
        string externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/unsettled/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> AdjustResettledBetManualAsync(
        Guid walletId,
        decimal amount,
        string description,
        string externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/adjust/resettled/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GamePlaceBetAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/game/bet/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameSettleWinAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/game/settle-win/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> DividendAllAgentRebateAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/dividend/allAgentRebate/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> PromotionVipLevelUpgradeAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/promotion/vip/upgrad/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> PromotionVipLevelSeasonAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/promotion/vip/season/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> PromotionVipLevelBirthdayAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/promotion/vip/birthday/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> AdjustAgentAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/adjust/agent/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> AdjustSystemAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/adjust/system/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> AdjustTransferAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default)
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
            .PutAsync($"/wallet/adjust/transfer/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameSettleLoseAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/game/settle-lose/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameSettleTieAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/game/settle-tie/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameSettleFreeAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/game/settle-free/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameCancelTransactionAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/game/cancel/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameLockAmountAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/game/lock/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameUnlockAmountAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/game/unlock/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameWinJackpotAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/game/win-jackpot/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }
}
