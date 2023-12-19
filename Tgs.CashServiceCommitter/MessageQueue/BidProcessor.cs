using Google.Protobuf;
using Tgs.Cash.Protos;
using Valhalla.MessageQueue;

namespace Tgs.CashServiceCommitter.MessageQueue;

public class BidProcessor(IDistributedResourceManager distributedResourceManager) : IMessageProcessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>
{
    public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var request = CommitExchangeRequest.Parser.ParseFrom(data.Span);

        var response = new BidResponse
        {
            Subject = await distributedResourceManager.BidAsync(
                request.WalletId,
                Environment.MachineName,
                cancellationToken).ConfigureAwait(false)
        };

        return response.ToByteArray();
    }
}
