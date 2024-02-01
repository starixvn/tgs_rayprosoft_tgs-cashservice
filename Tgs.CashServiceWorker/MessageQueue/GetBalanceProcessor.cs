using Google.Protobuf;
using StackExchange.Redis;
using Tgs.Cash.Protos;
using Tgs.CashServiceWorker.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.CashServiceWorker.MessageQueue;

public class GetBalanceProcessor(
    IGetService<GetBalanceQuery, decimal> getBalanceService,
    RedisConnectionManager redisConnectionManager,
    ILogger<GetBalanceProcessor> logger)
    : IMessageProcessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>
{
    private readonly IGetService<GetBalanceQuery, decimal> _getBalanceService = getBalanceService
        ?? throw new ArgumentNullException(nameof(getBalanceService));

    public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(
        string subject,
        ReadOnlyMemory<byte> data,
        IEnumerable<MessageHeaderValue>? headerValues,
        CancellationToken cancellationToken = default)
    {
        var request = BalanceRequest.Parser.ParseFrom(data.Span);
        var redisDatabase = redisConnectionManager.Database;
        var redisKey = new RedisKey($"Tgs:Cash:{request.WalletId.ToGuid():D}");

        var subjectKey = await redisDatabase.StringGetAsync(redisKey).ConfigureAwait(false);

        if (!subjectKey.HasValue || (string?)subjectKey != Environment.MachineName)
            logger.LogWarning("This machine ({MachineName}) not match {subjectKey}", Environment.MachineName, subjectKey);
        else
            _ = await redisDatabase
                .KeyExpireAsync(redisKey, TimeSpan.FromMinutes(10D), CommandFlags.FireAndForget)
                .ConfigureAwait(false);

        var response = new BalanceResponse
        {
            Balance = await _getBalanceService.GetAsync(
                new GetBalanceQuery(request.WalletId),
                cancellationToken).ConfigureAwait(false)
        };

        return response.ToByteArray();
    }
}
