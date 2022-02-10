using Google.Protobuf;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.Cash.Protos;
using Tgs.MessageQueue;

namespace Tgs.CashService.Core;

internal class WalletExchangeService : IWalletExchangeService
{
    private readonly IDatabase _redisDatabase;
    private readonly IMessageSender _messageSender;
    private readonly ILogger<WalletExchangeService> _logger;

    public WalletExchangeService(IDatabase redisDatabase, IMessageSender messageSender, ILogger<WalletExchangeService> logger)
    {
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<ExchangeResult> SendRequestAsync(WalletExchangeRequest request, CancellationToken cancellationToken)
    {
        var processorKey = $"Tgs:Cash:{request.WalletId.ToGuid():D}";
        _logger.LogInformation(processorKey);

        var processorSubjectValue = await _redisDatabase
            .StringGetAsync(processorKey)
            .ConfigureAwait(false);

        string subject;
        if (processorSubjectValue.HasValue)
        {
            subject = processorSubjectValue;
        }
        else
        {
            _logger.LogInformation("Start Bid");
            var bidCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            bidCts.CancelAfter(3000);

            var bidResponseData = await _messageSender.RequestAsync(
                "tgs.cash.bid",
                new BidRequest
                {
                    WalletId = request.WalletId
                }.ToByteArray(),
                bidCts.Token).ConfigureAwait(false);

            var bidResponse = BidResponse.Parser.ParseFrom(bidResponseData.Span);

            subject = bidResponse.Subject;
            _logger.LogInformation($"Subject is {subject}");
        }

        var exchangeAnswer = await _messageSender
            .AskAsync(subject, request.ToByteArray(), cancellationToken)
            .ConfigureAwait(false);
        var response = WalletExchangeResponse.Parser.ParseFrom(exchangeAnswer.Result.Span);
        await exchangeAnswer
            .CompleteAsync(Array.Empty<byte>(), cancellationToken)
            .ConfigureAwait(false);

        return new ExchangeResult(response.TransactionId, response.Balance);
    }
}
