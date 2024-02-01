using Google.Protobuf;
using Tgs.Cash.Protos;
using Valhalla.MessageQueue;

namespace Tgs.CashServiceCommitter.MessageQueue;

public class BidProcessor(IDistributedResourceManager distributedResourceManager)
    : IMessageProcessor<CommitExchangeRequest, BidResponse>
{
    public async ValueTask<BidResponse> HandleAsync(
        string subject,
        CommitExchangeRequest data,
        IEnumerable<MessageHeaderValue>? headerValues,
        CancellationToken cancellationToken = default)
    {
        var response = new BidResponse
        {
            Subject = await distributedResourceManager.BidAsync(
                data.WalletId,
                Environment.MachineName,
                cancellationToken).ConfigureAwait(false)
        };

        return response;
    }
}
