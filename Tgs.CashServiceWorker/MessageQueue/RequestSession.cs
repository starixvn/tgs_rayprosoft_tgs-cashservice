using Google.Protobuf;
using StackExchange.Redis;
using Tgs.Cash.Protos;
using Tgs.CashService;
using Tgs.CashServiceWorker.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.CashServiceWorker.MessageQueue;

internal class RequestSession : IMessageSession<ReadOnlyMemory<byte>>
{
    private readonly IGetService<ExchangeWalletCommand, ExchangeResult> _exchangeService;
    private readonly ILogger<RequestSession> _logger;
    private readonly IDatabase _redisDatabase;

    public RequestSession(
        IGetService<ExchangeWalletCommand, ExchangeResult> exchangeService,
        RedisConnectionManager redisConnectionManager,
        ILogger<RequestSession> logger)
    {
        ArgumentNullException.ThrowIfNull(redisConnectionManager);

        _exchangeService = exchangeService ?? throw new ArgumentNullException(nameof(exchangeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisDatabase = redisConnectionManager.Database;
    }

    public async ValueTask HandleAsync(Question<ReadOnlyMemory<byte>> question, CancellationToken cancellationToken = default)
    {
        var request = WalletExchangeRequest.Parser.ParseFrom(question.Data.Span);
        var redisKey = new RedisKey($"Tgs:Cash:{request.WalletId.ToGuid():D}");

        var subjectKey = await _redisDatabase.StringGetAsync(redisKey).ConfigureAwait(false);

        if (!subjectKey.HasValue || (string?)subjectKey != Environment.MachineName)
            _logger.LogWarning("This machine ({MachineName}) not match {subjectKey}", Environment.MachineName, subjectKey);
        else
            _ = await _redisDatabase
                .KeyExpireAsync(redisKey, TimeSpan.FromMinutes(10D), CommandFlags.FireAndForget)
                .ConfigureAwait(false);

        var result = await _exchangeService.GetAsync(
            new ExchangeWalletCommand(
                request.WalletId,
                Enum.Parse<TransactionType>(request.TransactionType, true),
                request.Amount,
                request.LockAmount,
                request.Description,
                request.ExternalId,
                request.CreatedUser),
            cancellationToken).ConfigureAwait(false);

        var response = new WalletExchangeResponse
        {
            TransactionId = result.TransactionId,
            Balance = result.Balance
        };

        _ = await question.AskAsync<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>(
            response.ToByteArray(),
            cancellationToken)
            .AsTask()
            .WaitAsync(TimeSpan.FromSeconds(5), cancellationToken)
            .ConfigureAwait(false);
    }
}
