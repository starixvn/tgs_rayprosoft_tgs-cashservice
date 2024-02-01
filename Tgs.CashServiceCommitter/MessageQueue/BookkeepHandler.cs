using Tgs.Cash.Protos;
using Valhalla.MessageQueue;
using Valhalla.Messages;

namespace Tgs.CashServiceCommitter.MessageQueue;

public class BookkeepHandler(IEventBus eventBus) : IMessageHandler<ReadOnlyMemory<byte>>
{
    private readonly IEventPublisher<WalletChangedEvent> _walletChangedEventSource =
        eventBus.GetEventPublisher<WalletChangedEvent>(CommitterConsts.WalletChangedEventName)!;

    public async ValueTask HandleAsync(
        string subject,
        ReadOnlyMemory<byte> data,
        IEnumerable<MessageHeaderValue>? headerValues,
        CancellationToken cancellationToken = default)
    {
        var request = CommitExchangeRequest.Parser.ParseFrom(data.Span);

        _ = await _walletChangedEventSource.PublishAsync(
            new WalletChangedEvent(request.WalletId),
            cancellationToken).ConfigureAwait(false);
    }
}
