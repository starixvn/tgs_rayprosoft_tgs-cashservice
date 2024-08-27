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

    public async ValueTask<TransactionResponse> GameProviderAddBalanceAsync(
        string playerCode,
        decimal amount,
        Guid? walletId,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            WalletId = walletId,
            Description = description,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/gp/add-balance/{playerCode}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameProviderDeductBalanceAsync(
        string playerCode,
        decimal amount,
        Guid? walletId,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            Amount = amount,
            WalletId = walletId,
            Description = description,
            ExternalId = externalId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

        var response = await _httpClient
            .PutAsync($"/wallet/gp/deduct-balance/{playerCode}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }
}
