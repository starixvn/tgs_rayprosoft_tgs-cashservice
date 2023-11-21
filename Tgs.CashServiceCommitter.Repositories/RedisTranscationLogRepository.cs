using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;
using Tgs.CashService;

namespace Tgs.CashServiceCommitter.Repositories;

internal class RedisTranscationLogRepository(IDatabase redisDatabase) : ITranscationLogRepository
{
    private static readonly JsonSerializerOptions _SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = false,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public IAsyncEnumerable<TransactionRecord> GetWaitingBookkeepLogsAsync(Guid walletId, CancellationToken cancellationToken = default)
        => redisDatabase.ListRangeAsync($"Tgs:Transactions:{walletId}", 0, 10)
            .ToAsyncEnumerable()
            .SelectMany(values => values.ToAsyncEnumerable())
            .Select(value => JsonSerializer.Deserialize<TransactionRecord>((string)value!, _SerializerOptions))
            .Where(record => record is not null)
            .Select(record => record!)
            .ToArrayAsync(cancellationToken: cancellationToken)
            .AsTask()
            .ToAsyncEnumerable()
            .SelectMany(arr => arr.ToAsyncEnumerable());

    public async ValueTask RemoveBookkeepLogsAsync(Guid walletId, int length, CancellationToken cancellationToken = default)
        => await redisDatabase.ListTrimAsync(
            $"Tgs:Transactions:{walletId}",
            length,
            -1).ConfigureAwait(false);
}
