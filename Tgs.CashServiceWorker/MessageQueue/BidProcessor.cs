using Google.Protobuf;
using StackExchange.Redis;
using Tgs.Cash.Protos;
using Tgs.MessageQueue;

namespace Tgs.CashServiceWorker.MessageQueue;

internal class BidProcessor : IMessageProcessor
{
    private readonly IDatabase _redisDatabase;

    public BidProcessor(RedisConnectionManager redisConnectionManager)
    {
        if (redisConnectionManager is null)
            throw new ArgumentNullException(nameof(redisConnectionManager));

        _redisDatabase = redisConnectionManager.Database;
    }

    public async ValueTask<byte[]> HandleAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var request = BidRequest.Parser.ParseFrom(data);

        var redisKey = new RedisKey($"Tgs:Cash:{request.WalletId.ToGuid():D}");

        var isGot = await _redisDatabase.StringSetAsync(
            redisKey,
            "test",
            expiry: TimeSpan.FromMinutes(1),
            when: When.NotExists).ConfigureAwait(false);

        var response = new BidResponse();

        if (isGot)
            response.Subject = $"tgs.cash.worker.{Environment.MachineName}";
        else
        {
            var subject = await _redisDatabase
                .StringGetAsync(redisKey)
                .ConfigureAwait(false);

            response.Subject = subject;
        }

        return response.ToByteArray();
    }
}
