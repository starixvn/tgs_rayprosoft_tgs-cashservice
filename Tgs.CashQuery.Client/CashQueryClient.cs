using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.CashQueryService;

namespace Tgs.CashQuery.Client;

internal class CashQueryClient
{
    private static readonly JsonSerializerOptions _SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private readonly HttpClient _httpClient;

    public CashQueryClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public IAsyncEnumerable<AdjustRecordListItem> QueryPlayerAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int skip = 0,
        int take = 10,
        CancellationToken cancellationToken = default)
    {
        var queryStrings = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("from", condition.CreatedOnRange.From.ToUniversalTime().ToString("O")),
            new KeyValuePair<string, string>("to", condition.CreatedOnRange.To.ToUniversalTime().ToString("O")),
            new KeyValuePair<string, string>("skip", skip.ToString()),
            new KeyValuePair<string, string>("take", take.ToString())
        };

        if (condition.SearchTypes is not null && condition.SearchTypes.Any())
            foreach (var type in condition.SearchTypes)
                queryStrings.Add(new KeyValuePair<string, string>("type", type.ToString()));

        if (!string.IsNullOrEmpty(condition.UserId))
            queryStrings.Add(new KeyValuePair<string, string>("userId", condition.UserId));

        if (!string.IsNullOrEmpty(condition.Operator))
            queryStrings.Add(new KeyValuePair<string, string>("operator", condition.Operator));

        return _httpClient.GetAsync(
            $"/adjust/player/{brandId}?{string.Join(
                "&",
                queryStrings.Select(
                    kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"))}",
            cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Content
                .ReadFromJsonAsync<IAsyncEnumerable<AdjustRecordListItem>>(_SerializerOptions, cancellationToken)
                .ToAsyncEnumerable()
                .Where(arr => arr is not null)
                .SelectMany(arr => arr!))
            .Select(record => record with { UserType = UserType.Player });
    }

    public IAsyncEnumerable<AdjustRecordListItem> QueryAgentAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int skip = 0,
        int take = 10,
        CancellationToken cancellationToken = default)
    {
        var queryStrings = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("from", condition.CreatedOnRange.From.ToUniversalTime().ToString("O")),
            new KeyValuePair<string, string>("to", condition.CreatedOnRange.To.ToUniversalTime().ToString("O")),
            new KeyValuePair<string, string>("skip", skip.ToString()),
            new KeyValuePair<string, string>("take", take.ToString())
        };

        if (condition.SearchTypes is not null && condition.SearchTypes.Any())
            foreach (var type in condition.SearchTypes)
                queryStrings.Add(new KeyValuePair<string, string>("type", type.ToString()));

        if (!string.IsNullOrEmpty(condition.UserId))
            queryStrings.Add(new KeyValuePair<string, string>("userId", condition.UserId));

        if (!string.IsNullOrEmpty(condition.Operator))
            queryStrings.Add(new KeyValuePair<string, string>("operator", condition.Operator));

        return _httpClient.GetAsync(
            $"/adjust/agent/{brandId}?{string.Join(
                "&",
                queryStrings.Select(
                    kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"))}",
            cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Content
                .ReadFromJsonAsync<IAsyncEnumerable<AdjustRecordListItem>>(_SerializerOptions, cancellationToken)
                .ToAsyncEnumerable()
                .Where(arr => arr is not null)
                .SelectMany(arr => arr!))
            .Select(record => record with { UserType = UserType.Player });
    }

    public IAsyncEnumerable<AdjustSummaryListItem> QueryPlayerAdjustSummariesAsync(
        AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default)
    {
        var queryStrings = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("from", condition.CreatedOnRange.From.ToUniversalTime().ToString("O")),
            new KeyValuePair<string, string>("to", condition.CreatedOnRange.To.ToUniversalTime().ToString("O"))
        };

        if (condition.SearchTypes is not null && condition.SearchTypes.Any())
            foreach (var type in condition.SearchTypes)
                queryStrings.Add(new KeyValuePair<string, string>("type", type.ToString()));

        if (!string.IsNullOrEmpty(condition.UserId))
            queryStrings.Add(new KeyValuePair<string, string>("userId", condition.UserId));

        if (!string.IsNullOrEmpty(condition.Operator))
            queryStrings.Add(new KeyValuePair<string, string>("operator", condition.Operator));

        return _httpClient.GetAsync(
            $"/adjust/summary/player?{string.Join(
                "&",
                queryStrings.Select(
                    kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"))}",
            cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Content
                .ReadFromJsonAsync<IAsyncEnumerable<AdjustSummaryListItem>>(_SerializerOptions, cancellationToken)
                .ToAsyncEnumerable()
                .Where(arr => arr is not null)
                .SelectMany(arr => arr!));
    }

    public IAsyncEnumerable<AdjustSummaryListItem> QueryAgentAdjustSummariesAsync(
        AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default)
    {
        var queryStrings = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("from", condition.CreatedOnRange.From.ToUniversalTime().ToString("O")),
            new KeyValuePair<string, string>("to", condition.CreatedOnRange.To.ToUniversalTime().ToString("O"))
        };

        if (condition.SearchTypes is not null && condition.SearchTypes.Any())
            foreach (var type in condition.SearchTypes)
                queryStrings.Add(new KeyValuePair<string, string>("type", type.ToString()));

        if (!string.IsNullOrEmpty(condition.UserId))
            queryStrings.Add(new KeyValuePair<string, string>("userId", condition.UserId));

        if (!string.IsNullOrEmpty(condition.Operator))
            queryStrings.Add(new KeyValuePair<string, string>("operator", condition.Operator));

        return _httpClient.GetAsync(
            $"/adjust/summary/agent?{string.Join(
                "&",
                queryStrings.Select(
                    kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"))}",
            cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Content
                .ReadFromJsonAsync<IAsyncEnumerable<AdjustSummaryListItem>>(_SerializerOptions, cancellationToken)
                .ToAsyncEnumerable()
                .Where(arr => arr is not null)
                .SelectMany(arr => arr!));
    }
}
