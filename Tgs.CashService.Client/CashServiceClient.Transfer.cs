using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;

namespace Tgs.CashService.Client;

internal partial class CashServiceClient
{
    public ValueTask<TransactionResponse> LockTransferAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid? executor,
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
        if (executor is not null)
            requestContent.Headers.Add("Modifier", executor.Value.ToString());

        return _httpClient
            .PostAsync($"/wallet/transfer-lock/{walletId}", requestContent, cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Content
                .ReadFromJsonAsync<TransactionResponse>(_SerializerOptions, cancellationToken)
                .ToAsyncEnumerable())
            .Where(result => result is not null)
            .Select(result => result!)
            .FirstAsync(cancellationToken);
    }

    public ValueTask<TransactionResponse> CommitTransferAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid? executor,
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
        if (executor is not null)
            requestContent.Headers.Add("Modifier", executor.Value.ToString());

        return _httpClient
            .PutAsync($"/wallet/transfer/{walletId}", requestContent, cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Content
                .ReadFromJsonAsync<TransactionResponse>(_SerializerOptions, cancellationToken)
                .ToAsyncEnumerable())
            .Where(result => result is not null)
            .Select(result => result!)
            .FirstAsync(cancellationToken);
    }

    public async ValueTask<TransactionResponse> RollbackTransferAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid? executor,
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
        if (executor is not null)
            requestContent.Headers.Add("Modifier", executor.Value.ToString());

        using var requestMessage = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/wallet/transfer/{walletId}")
        {
            Content = requestContent
        };

        return await _httpClient
            .SendAsync(requestMessage, cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Content
                .ReadFromJsonAsync<TransactionResponse>(_SerializerOptions, cancellationToken)
                .ToAsyncEnumerable())
            .Where(result => result is not null)
            .Select(result => result!)
            .FirstAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public ValueTask<TransactionResponse> LockReceiveAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid? executor,
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
        if (executor is not null)
            requestContent.Headers.Add("Modifier", executor.Value.ToString());

        return _httpClient
            .PostAsync($"/wallet/receive-lock/{walletId}", requestContent, cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Content
                .ReadFromJsonAsync<TransactionResponse>(_SerializerOptions, cancellationToken)
                .ToAsyncEnumerable())
            .Where(result => result is not null)
            .Select(result => result!)
            .FirstAsync(cancellationToken);
    }

    public ValueTask<TransactionResponse> CommitReceiveAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid? executor,
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
        if (executor is not null)
            requestContent.Headers.Add("Modifier", executor.Value.ToString());

        return _httpClient
            .PutAsync($"/wallet/receive/{walletId}", requestContent, cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Content
                .ReadFromJsonAsync<TransactionResponse>(_SerializerOptions, cancellationToken)
                .ToAsyncEnumerable())
            .Where(result => result is not null)
            .Select(result => result!)
            .FirstAsync(cancellationToken);
    }

    public async ValueTask<TransactionResponse> RollbackReceiveAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid? executor,
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
        if (executor is not null)
            requestContent.Headers.Add("Modifier", executor.Value.ToString());

        using var requestMessage = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/wallet/receive/{walletId}")
        {
            Content = requestContent
        };

        return await _httpClient
            .SendAsync(requestMessage, cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Content
                .ReadFromJsonAsync<TransactionResponse>(_SerializerOptions, cancellationToken)
                .ToAsyncEnumerable())
            .Where(result => result is not null)
            .Select(result => result!)
            .FirstAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
