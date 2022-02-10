using Tgs.MessageQueue;

namespace Tgs.CashServiceWorker.MessageQueue;

public class GetBalanceProcessor : IMessageProcessor
{
    public ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
