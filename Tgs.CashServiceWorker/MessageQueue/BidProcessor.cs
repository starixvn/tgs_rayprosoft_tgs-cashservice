using Google.Protobuf;
using StackExchange.Redis;
using Tgs.Cash.Protos;
using Tgs.MessageQueue;

namespace Tgs.CashServiceWorker.MessageQueue;

internal class BidProcessor : IMessageProcessor
{
    private readonly IDatabase _redisDatabase;
    private readonly ILogger<BidProcessor> _logger;

    public BidProcessor(RedisConnectionManager redisConnectionManager, ILogger<BidProcessor> logger)
    {
        if (redisConnectionManager is null)
            throw new ArgumentNullException(nameof(redisConnectionManager));

        _redisDatabase = redisConnectionManager.Database;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var request = BidRequest.Parser.ParseFrom(data.Span);

        var redisKey = new RedisKey($"Tgs:Cash:{request.WalletId.ToGuid():D}");

        var isGot = await _redisDatabase.StringSetAsync(
            redisKey,
            Environment.MachineName,
            expiry: TimeSpan.FromMinutes(1),
            when: When.NotExists).ConfigureAwait(false);

        var response = new BidResponse();

        if (isGot)
            response.Subject = Environment.MachineName;
        else
        {
            var subject = await _redisDatabase
                .StringGetAsync(redisKey)
                .ConfigureAwait(false);

            response.Subject = subject;
        }

        _logger.LogInformation($"Subject: {response.Subject}");

        return response.ToByteArray();
    }
}
