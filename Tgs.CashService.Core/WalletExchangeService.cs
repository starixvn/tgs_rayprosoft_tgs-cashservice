using Google.Protobuf;
using StackExchange.Redis;
using Tgs.Cash.Protos;
using Tgs.CashServiceWorker;
using Tgs.MessageQueue;

namespace Tgs.CashService.Core;

internal class WalletExchangeService : IWalletExchangeService
{
    private readonly IDatabase _redisDatabase;
    private readonly IMessageSender _messageSender;

    public WalletExchangeService(IDatabase redisDatabase, IMessageSender messageSender)
    {
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
    }

    public async ValueTask<ExchangeResult> SendRequestAsync(WalletExchangeRequest request)
    {
        var processorKey = $"Tgs:Cash:{request.WalletId.ToGuid():D}";

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
            var bidResponseData = await _messageSender.RequestAsync(
                "tgs.cash.bid",
                new BidRequest
                {
                    WalletId = request.WalletId
                }.ToByteArray()).ConfigureAwait(false);

            var bidResponse = BidResponse.Parser.ParseFrom(bidResponseData);

            subject = bidResponse.Subject;
        }

        var responseData = await _messageSender
            .RequestAsync(subject, request.ToByteArray())
            .ConfigureAwait(false);
        var response = WalletExchangeResponse.Parser.ParseFrom(responseData);

        return new ExchangeResult(response.TransactionId, response.Balance);
    }
}
