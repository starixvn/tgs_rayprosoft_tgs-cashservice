using Tgs.Cash.Protos;
using Valhalla.MessageQueue;
using Valhalla.Messages;

namespace Tgs.CashServiceCommitter.MessageQueue;

public class BookkeepHandler(IEventBus eventBus) : IMessageHandler<CommitExchangeRequest>
{
    private readonly IEventPublisher<WalletChangedEvent> _walletChangedEventSource =
        eventBus.GetEventPublisher<WalletChangedEvent>(CommitterConsts.WalletChangedEventName)!;

    public async ValueTask HandleAsync(
        string subject,
        CommitExchangeRequest data,
        IEnumerable<MessageHeaderValue>? headerValues,
        CancellationToken cancellationToken = default)
        => await _walletChangedEventSource.PublishAsync(
            new WalletChangedEvent(data.WalletId),
            cancellationToken).ConfigureAwait(false);
}
