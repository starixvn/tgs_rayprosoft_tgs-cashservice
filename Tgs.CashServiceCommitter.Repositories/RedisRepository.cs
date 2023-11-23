using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;
using Tgs.CashService;

namespace Tgs.CashServiceCommitter.Repositories;

internal class RedisRepository(IDatabase redisDatabase)
    : ITransactionLogRepository
    , IDistributedResourceManager
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

    public async ValueTask<string> BidAsync(Guid walletId, string token, CancellationToken cancellationToken = default)
    {
        var redisKey = new RedisKey($"Tgs:CashCommit:{walletId}");

        var isGot = await redisDatabase.StringSetAsync(
            redisKey,
            token,
            expiry: TimeSpan.FromMinutes(10D),
            when: When.NotExists).ConfigureAwait(false);

        return isGot
            ? token
            : (string?)await redisDatabase
                .StringGetAsync(redisKey)
                .ConfigureAwait(false) ?? string.Empty;
    }

    public async ValueTask DiscardBidAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var redisKey = new RedisKey($"Tgs:CashCommit:{walletId}");

        _ = await redisDatabase.KeyDeleteAsync(redisKey).ConfigureAwait(false);
    }

    public async ValueTask ExtendBidAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var redisKey = new RedisKey($"Tgs:CashCommit:{walletId}");

        _ = await redisDatabase.KeyExpireAsync(
            redisKey,
            TimeSpan.FromMinutes(10D)).ConfigureAwait(false);
    }

    public async ValueTask<string?> GetBidTokenAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var redisKey = new RedisKey($"Tgs:CashCommit:{walletId}");

        var token = await redisDatabase
            .StringGetAsync(redisKey)
            .ConfigureAwait(false);

        return token;
    }

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
