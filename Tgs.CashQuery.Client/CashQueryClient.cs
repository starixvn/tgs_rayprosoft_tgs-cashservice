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

        var requestPath = $"/adjust/player/{brandId}?{string.Join(
            "&",
            queryStrings.Select(
                kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"))}";
        var countRequest = new HttpRequestMessage(HttpMethod.Head, $"{requestPath}&skip={skip}");

        /*
         * 後端 api 最多回傳 1000 筆資料，先查詢總筆數，
         * 利用延遲特性在讀取該分頁時再查詢資料
         */
        return _httpClient.SendAsync(countRequest, cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Headers
                .GetValues("Tgs-Return")
                .ToAsyncEnumerable()
                .Take(1))
            .Select(int.Parse)
            .SelectMany(count => AsyncEnumerable.Range(0, (int)Math.Ceiling(count / 1000D))
                .SelectMany(idx => _httpClient.GetAsync($"{requestPath}&skip={skip + idx * 1000}")
                    .ToAsyncEnumerable()
                    .Do(response => response.EnsureSuccessStatusCode())
                    .SelectMany(response => response.Content
                        .ReadFromJsonAsync<IAsyncEnumerable<AdjustRecordListItem>>(_SerializerOptions, cancellationToken)
                        .ToAsyncEnumerable()
                        .Where(arr => arr is not null)
                        .SelectMany(arr => arr!))))
            .Select(record => record with { UserType = UserType.Player });
    }

    public IAsyncEnumerable<AdjustRecordListItem> QueryAgentAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int skip = 0,
        CancellationToken cancellationToken = default)
    {
        var queryStrings = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("from", condition.CreatedOnRange.From.ToUniversalTime().ToString("O")),
            new KeyValuePair<string, string>("to", condition.CreatedOnRange.To.ToUniversalTime().ToString("O")),
        };

        if (condition.SearchTypes is not null && condition.SearchTypes.Any())
            foreach (var type in condition.SearchTypes)
                queryStrings.Add(new KeyValuePair<string, string>("type", type.ToString()));

        if (!string.IsNullOrEmpty(condition.UserId))
            queryStrings.Add(new KeyValuePair<string, string>("userId", condition.UserId));

        if (!string.IsNullOrEmpty(condition.Operator))
            queryStrings.Add(new KeyValuePair<string, string>("operator", condition.Operator));

        var requestPath = $"/adjust/agent/{brandId}?{string.Join(
            "&",
            queryStrings.Select(
                kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"))}";
        var countRequest = new HttpRequestMessage(HttpMethod.Head, $"{requestPath}&skip={skip}");

        return _httpClient.SendAsync(countRequest, cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Headers
                .GetValues("Tgs-Return")
                .ToAsyncEnumerable()
                .Take(1))
            .Select(int.Parse)
            .SelectMany(count => AsyncEnumerable.Range(0, count)
                .SelectMany(idx => _httpClient.GetAsync($"{requestPath}&skip={skip + idx * 1000}")
                    .ToAsyncEnumerable()
                    .Do(response => response.EnsureSuccessStatusCode())
                    .SelectMany(response => response.Content
                        .ReadFromJsonAsync<IAsyncEnumerable<AdjustRecordListItem>>(_SerializerOptions, cancellationToken)
                        .ToAsyncEnumerable()
                        .Where(arr => arr is not null)
                        .SelectMany(arr => arr!))))
            .Select(record => record with { UserType = UserType.Agent });
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
