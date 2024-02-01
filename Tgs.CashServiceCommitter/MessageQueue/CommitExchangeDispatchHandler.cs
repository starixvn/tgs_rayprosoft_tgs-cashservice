using NATS.Client.Core;
using Polly;
using Tgs.Cash.Protos;
using Valhalla.MessageQueue;

namespace Tgs.CashServiceCommitter.MessageQueue;

public class CommitExchangeDispatchHandler(
    IMessageSender messageSender,
    IDistributedResourceManager distributedResourceManager,
    ILogger<CommitExchangeDispatchHandler> logger)
    : IAcknowledgeMessageHandler<CommitExchangeRequest>
{
    public async ValueTask HandleAsync(IAcknowledgeMessage<CommitExchangeRequest> msg, CancellationToken cancellationToken = default)
    {
        var request = msg.Data!;

        await Policy
            .Handle<NatsNoRespondersException>()
            .RetryAsync(
                1,
                async (ex, count) =>
                {
                    await distributedResourceManager.DiscardBidAsync(
                        request.WalletId.ToGuid(),
                        cancellationToken).ConfigureAwait(false);

                    logger.LogWarning(ex, "NoReponders error retry {count}", count);
                })
            .ExecuteAsync(
                async ct =>
                {
                    var bidToken = await GetBidTokenAsync(
                        request.WalletId,
                        cancellationToken).ConfigureAwait(false);

                    var subject = $"tgs.cash.commit.batch.{bidToken}";
                    await messageSender.SendAsync(
                        subject,
                        msg.Data,
                        cancellationToken).ConfigureAwait(false);
                },
                cancellationToken)
            .ConfigureAwait(false);

        await msg.AckAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    protected async Task<string> GetBidTokenAsync(Guid walletId, CancellationToken cancellationToken)
    {
        var bidToken = await distributedResourceManager
             .GetBidTokenAsync(walletId, cancellationToken)
             .ConfigureAwait(false);

        if (string.IsNullOrEmpty(bidToken))
        {
            var bidResponse = await messageSender.RequestAsync<CommitExchangeRequest, BidResponse>(
                "tgs.cash.commit.bid",
                new CommitExchangeRequest
                {
                    WalletId = walletId
                },
                cancellationToken)
                .ConfigureAwait(false);

            bidToken = bidResponse.Subject;
        }

        return bidToken;
    }
}
