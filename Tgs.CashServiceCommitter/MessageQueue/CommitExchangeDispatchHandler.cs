using Google.Protobuf;
using NATS.Client.Core;
using Polly;
using Tgs.Cash.Protos;
using Valhalla.MessageQueue;

namespace Tgs.CashServiceCommitter.MessageQueue;

public class CommitExchangeDispatchHandler(
    IMessageSender messageSender,
    IDistributedResourceManager distributedResourceManager,
    ILogger<CommitExchangeDispatchHandler> logger)
    : IAcknowledgeMessageHandler<ReadOnlyMemory<byte>>
{
    public async ValueTask HandleAsync(IAcknowledgeMessage<ReadOnlyMemory<byte>> msg, CancellationToken cancellationToken = default)
    {
        var request = CommitExchangeRequest.Parser.ParseFrom(msg.Data.Span);

        var bidToken = await GetBidTokenAsync(
            request.WalletId,
            cancellationToken).ConfigureAwait(false);

        var subject = $"tgs.cash.commit.batch.{bidToken}";

        await messageSender.SendAsync(
            subject,
            msg.Data,
            cancellationToken).ConfigureAwait(false);

        await msg.AckAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    protected Task<string> GetBidTokenAsync(Guid walletId, CancellationToken cancellationToken)
        => Policy
            .Handle<NatsNoRespondersException>()
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
            var bidResponseData = await messageSender.RequestAsync<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>(
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
