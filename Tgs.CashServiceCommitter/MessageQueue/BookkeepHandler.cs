using Tgs.Cash.Protos;
using Valhalla.MessageQueue;
using Valhalla.Messages;

namespace Tgs.CashServiceCommitter.MessageQueue;

public class BookkeepHandler(IEventBus eventBus) : IMessageHandler
{
    private readonly IEventSource<WalletChangedEvent> _walletChangedEventSource =
        eventBus.GetEventSource<WalletChangedEvent>(CommitterConsts.WalletChangedEventName)!;

    public async ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var request = CommitExchangeRequest.Parser.ParseFrom(data.Span);
        _ = await _walletChangedEventSource.PublishAsync(
            new WalletChangedEvent(request.WalletId),
            cancellationToken).ConfigureAwait(false);
    }
}
