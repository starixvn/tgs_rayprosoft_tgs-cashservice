namespace Tgs.CashServiceWorker;

internal interface IWalletExchangeContext
{
    Guid WalletId { get; }

    Guid BrandId { get; }

    string UserId { get; }

    decimal Balance { get; set; }

    decimal LockedBalance { get; set; }

    ValueTask NotifyCommitAsync(CancellationToken cancellationToken = default);
}
