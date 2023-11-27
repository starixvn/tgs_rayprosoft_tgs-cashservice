using Google.Protobuf;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.Cash.Protos;
using Valhalla.MessageQueue;

namespace Tgs.CashService;

internal class WalletExchangeService(IDatabase redisDatabase, IMessageSender messageSender, ILogger<WalletExchangeService> logger) : IWalletExchangeService
{
    private readonly IDatabase _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
    private readonly IMessageSender _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
    private readonly ILogger<WalletExchangeService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async ValueTask<ExchangeResult> SendRequestAsync(WalletExchangeRequest request, CancellationToken cancellationToken)
    {
        var machineName = await GetProcessorSubjectAsync(
            request.WalletId,
            cancellationToken).ConfigureAwait(false);

        var exchangeAnswer = await _messageSender
            .AskAsync($"tgs.cash.worker.{machineName}", request.ToByteArray(), cancellationToken)
            .ConfigureAwait(false);
        var response = WalletExchangeResponse.Parser.ParseFrom(exchangeAnswer.Result.Span);
        await exchangeAnswer
            .CompleteAsync(Array.Empty<byte>(), cancellationToken)
            .ConfigureAwait(false);

        return new ExchangeResult(response.TransactionId, response.Balance, response.LockedBalance);
    }

    public async ValueTask<decimal?> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var machineName = await GetProcessorSubjectAsync(
            walletId,
            cancellationToken).ConfigureAwait(false);

        var request = new BalanceRequest
        {
            WalletId = walletId
        };

        var responseData = await _messageSender
            .RequestAsync($"tgs.cash.balance.{machineName}", request.ToByteArray(), cancellationToken)
            .ConfigureAwait(false);

        var response = BalanceResponse.Parser.ParseFrom(responseData.Span);

        return response.Balance;
    }

    public async ValueTask DiscardBidAsync(Guid walletId)
    {
        var processorKey = $"Tgs:Cash:{walletId:D}";

        _ = await _redisDatabase
            .KeyDeleteAsync(processorKey)
            .ConfigureAwait(false);
    }

    private async ValueTask<string> GetProcessorSubjectAsync(Guid walletId, CancellationToken cancellationToken)
    {
        var processorKey = $"Tgs:Cash:{walletId:D}";
        _logger.LogDebug("{processorKey}", processorKey);

        var processorSubjectValue = await _redisDatabase
            .StringGetAsync(processorKey)
            .ConfigureAwait(false);

        string machineName;
        if (processorSubjectValue.HasValue)
        {
            machineName = ((string?)processorSubjectValue)!;
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
                    WalletId = walletId
                }.ToByteArray(),
                bidCts.Token).ConfigureAwait(false);

            var bidResponse = BidResponse.Parser.ParseFrom(bidResponseData.Span);

            machineName = bidResponse.Subject;
            _logger.LogInformation("Subject is {MachineName}", machineName);
        }

        return machineName;
    }
}
