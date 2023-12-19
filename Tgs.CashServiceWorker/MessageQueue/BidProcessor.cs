using Google.Protobuf;
using StackExchange.Redis;
using Tgs.Cash.Protos;
using Tgs.CashServiceWorker.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.CashServiceWorker.MessageQueue;

internal class BidProcessor : IMessageProcessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>
{
    private readonly IDatabase _redisDatabase;
    private readonly ICommandService<DiscardReceptionCommand> _discardService;
    private readonly ILogger<BidProcessor> _logger;

    public BidProcessor(
        RedisConnectionManager redisConnectionManager,
        ICommandService<DiscardReceptionCommand> discardService,
        ILogger<BidProcessor> logger)
    {
        ArgumentNullException.ThrowIfNull(redisConnectionManager);

        _redisDatabase = redisConnectionManager.Database;
        _discardService = discardService ?? throw new ArgumentNullException(nameof(discardService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var request = BidRequest.Parser.ParseFrom(data.Span);

        await _discardService.ExecuteAsync(
            new DiscardReceptionCommand(request.WalletId),
            cancellationToken).ConfigureAwait(false);

        var redisKey = new RedisKey($"Tgs:Cash:{request.WalletId.ToGuid():D}");

        var isGot = await _redisDatabase.StringSetAsync(
            redisKey,
            Environment.MachineName,
            expiry: TimeSpan.FromMinutes(10D),
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

        _logger.LogInformation("Subject: {subject}", response.Subject);

        return response.ToByteArray();
    }
}
