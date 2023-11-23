namespace Tgs.CashServiceCommitter;

public interface IDistributedResourceManager
{
    ValueTask<string> BidAsync(Guid walletId, string token, CancellationToken cancellationToken = default);

    ValueTask<string?> GetBidTokenAsync(Guid walletId, CancellationToken cancellationToken = default);

    ValueTask DiscardBidAsync(Guid walletId, CancellationToken cancellationToken = default);

    ValueTask ExtendBidAsync(Guid walletId, CancellationToken cancellationToken = default);
}
