namespace Tgs.CashServiceWorker.Core.UnitTests.ExchangeCommands;

internal class StubWalletExchangeContext(
    Guid walletId,
    Guid brandId,
    string userId,
    decimal balance,
    decimal lockedBalance) : IWalletExchangeContext
{
    public Guid WalletId { get; } = walletId;

    public Guid BrandId { get; } = brandId;

    public string UserId { get; } = userId;

    public decimal Balance { get; set; } = balance;

    public decimal LockedBalance { get; set; } = lockedBalance;

    public ValueTask NotifyCommitAsync(CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
}
