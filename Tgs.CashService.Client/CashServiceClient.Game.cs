using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;

namespace Tgs.CashService.Client;

internal partial class CashServiceClient
{
    public async ValueTask<TransactionResponse> GameBetAsync(
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

    public async ValueTask<TransactionResponse> GameCancelAsync(
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

    public async ValueTask<TransactionResponse> GameLockAsync(
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

    public async ValueTask<TransactionResponse> GameUnlockAsync(
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

    public async ValueTask<TransactionResponse> GameCreditAsync(
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
            .PutAsync($"/wallet/game/credit/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameDebitAsync(
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
            .PutAsync($"/wallet/game/debit/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameRefundAsync(
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
            .PutAsync($"/wallet/game/refund/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }

    public async ValueTask<TransactionResponse> GameRevisionAsync(
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
            .PutAsync($"/wallet/game/revision/{walletId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<TransactionResponse>(contentJson, _SerializerOptions)!;
    }
}
