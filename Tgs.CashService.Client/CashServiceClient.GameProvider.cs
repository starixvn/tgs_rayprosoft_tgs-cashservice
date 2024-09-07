using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;

namespace Tgs.CashService.Client;

internal partial class CashServiceClient
{
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

    public async ValueTask<TransactionResponse> GameProviderFundRefundAsync(
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
            .PutAsync($"/wallet/gp/fund-refund/{walletId}", requestContent, cancellationToken)
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

    public async ValueTask<BalanceResponse> GameProviderGetBalanceAsync(string playerCode, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient
            .GetAsync($"/wallet/gp/get-balance/{playerCode}", cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<BalanceResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<BalanceResponse> GameProviderAddBalanceAsync(
        string playerCode,
        decimal amount,
        ProviderInfo providerInfo,
        string? description = "",
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            Description = description,
            providerInfo.ProviderCode,
            ExternalId = providerInfo.RefId,
            providerInfo.OperationId,
            providerInfo.Action
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/gp/add-balance/{playerCode}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<BalanceResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<BalanceResponse> GameProviderDeductBalanceAsync(
        string playerCode,
        decimal amount,
        ProviderInfo providerInfo,
        string? description = "",
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            Description = description,
            providerInfo.ProviderCode,
            ExternalId = providerInfo.RefId,
            providerInfo.OperationId,
            providerInfo.Action
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/gp/deduct-balance/{playerCode}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<BalanceResponse>(contentJson, _SerializerOptions)!;
    }
    public async ValueTask<BalanceResponse> GameProviderAdjustBalanceAsync(
        string playerCode,
        decimal addedAmount,
        decimal deductedAmount,
        ProviderInfo providerInfo,
        string? description = "",
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            AddedAmount = addedAmount,
            DeductedAmount = deductedAmount,
            Description = description,
            providerInfo.ProviderCode,
            ExternalId = providerInfo.RefId,
            providerInfo.OperationId,
            providerInfo.Action
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/gp/adjust-balance/{playerCode}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<BalanceResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<bool> IsExistGameProviderTransactionLogAsync(
        string playerCode,
        ProviderInfo providerInfo,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            providerInfo.ProviderCode,
            ExternalId = providerInfo.RefId,
            providerInfo.Action
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/gp/exist-transaction/{playerCode}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<bool>(contentJson, _SerializerOptions)!;
    }
}
