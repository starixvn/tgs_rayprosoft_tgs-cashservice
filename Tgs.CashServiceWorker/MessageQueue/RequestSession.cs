using Google.Protobuf;
using StackExchange.Redis;
using Tgs.Cash.Protos;
using Tgs.CashService;
using Tgs.CashServiceWorker.ApplicationServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.CashServiceWorker.MessageQueue;

internal class RequestSession : IMessageSession
{
    private readonly IGetService<ExchangeWalletCommand, ExchangeResult> _exchangeService;
    private readonly IDatabase _redisDatabase;

    public RequestSession(
        IGetService<ExchangeWalletCommand, ExchangeResult> exchangeService,
        RedisConnectionManager redisConnectionManager)
    {
        if (redisConnectionManager is null)
            throw new ArgumentNullException(nameof(redisConnectionManager));

        _exchangeService = exchangeService ?? throw new ArgumentNullException(nameof(exchangeService));
        _redisDatabase = redisConnectionManager.Database;
    }

    public async ValueTask HandleAsync(Question question, CancellationToken cancellationToken = default)
    {
        var request = WalletExchangeRequest.Parser.ParseFrom(question.Data.Span);
        var redisKey = new RedisKey($"Tgs:Cash:{request.WalletId.ToGuid():D}");

        var subjectKey = await _redisDatabase.StringGetAsync(redisKey).ConfigureAwait(false);

        if (!subjectKey.HasValue || (string)subjectKey != Environment.MachineName)
            throw new RequestInvalidSubjectException($"This machine ({Environment.MachineName}) not match {subjectKey}");

        _ = await _redisDatabase
            .KeyExpireAsync(redisKey, TimeSpan.FromMinutes(1), CommandFlags.FireAndForget)
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

        _ = await question
            .AskAsync(response.ToByteArray(), cancellationToken)
            .AsTask()
            .WaitAsync(TimeSpan.FromSeconds(5), cancellationToken)
            .ConfigureAwait(false);
    }
}
