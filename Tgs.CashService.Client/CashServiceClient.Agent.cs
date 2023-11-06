using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;

namespace Tgs.CashService.Client;

internal partial class CashServiceClient
{
    public async ValueTask<TransactionResponse> AgentDividendAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default)
        => await AsyncEnumerableEx
            .Return(new
            {
                Amount = amount,
                Description = description,
                ExternalId = externalId
            })
            .Select(request =>
            {
                var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
                requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

                return requestContent;
            })
            .SelectMany(requestContent => _httpClient
                .PutAsync($"/wallet/agent-dividend/{walletId}", requestContent, cancellationToken)
                .ToAsyncEnumerable())
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Content
                .ReadFromJsonAsync<TransactionResponse>(_SerializerOptions)
                .ToAsyncEnumerable())
            .Select(result => result!)
            .FirstAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
}
