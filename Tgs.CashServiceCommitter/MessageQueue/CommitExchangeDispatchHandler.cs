using Google.Protobuf;
using NATS.Client;
using Polly;
using Tgs.Cash.Protos;
using Valhalla.MessageQueue;
using Valhalla.MessageQueue.Nats;

namespace Tgs.CashServiceCommitter.MessageQueue;

public class CommitExchangeDispatchHandler(
    IMessageSender messageSender,
    IDistributedResourceManager distributedResourceManager,
    ILogger<CommitExchangeDispatchHandler> logger)
    : INatsMessageHandler
{
    public async ValueTask HandleAsync(MsgHandlerEventArgs args, CancellationToken cancellationToken = default)
    {
        var request = CommitExchangeRequest.Parser.ParseFrom(args.Message.Data);

        var bidToken = await GetBidTokenAsync(
            request.WalletId,
            cancellationToken).ConfigureAwait(false);

        var subject = $"tgs.cash.commit.batch.{bidToken}";

        await messageSender.PublishAsync(
            subject,
            args.Message.Data,
            cancellationToken).ConfigureAwait(false);

        args.Message.Ack();
    }

    protected Task<string> GetBidTokenAsync(Guid walletId, CancellationToken cancellationToken)
        => Policy
            .Handle<NATSNoRespondersException>()
            .RetryAsync(
                1,
                async (ex, count) =>
                {
                    await distributedResourceManager.DiscardBidAsync(
                        walletId,
                        cancellationToken).ConfigureAwait(false);

                    logger.LogWarning(ex, "NoReponders error retry {count}", count);
                })
            .ExecuteAsync(
                async ct => await InternalGetBidTokenAsync(
                    walletId,
                    cancellationToken).ConfigureAwait(false),
                cancellationToken);

    private async ValueTask<string> InternalGetBidTokenAsync(Guid walletId, CancellationToken cancellationToken)
    {
        var bidToken = await distributedResourceManager
            .GetBidTokenAsync(walletId, cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrEmpty(bidToken))
        {
            var bidResponseData = await messageSender.RequestAsync(
                "tgs.cash.commit.bid",
                new CommitExchangeRequest
                {
                    WalletId = walletId
                }.ToByteArray(),
                cancellationToken)
                .ConfigureAwait(false);

            var bidResponse = BidResponse.Parser.ParseFrom(bidResponseData.Span);

            bidToken = bidResponse.Subject;
        }

        return bidToken;
    }
}
