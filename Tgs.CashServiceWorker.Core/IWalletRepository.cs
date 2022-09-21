using Tgs.CashService;

namespace Tgs.CashServiceWorker;

public interface IWalletRepository
{
    ValueTask<ExchangeResult> ExchangeAsync(
        Guid brandId,
        Guid walletId,
        string playerCode,
        TransactionType transactionType,
        decimal amount,
        decimal lockAmount,
        string? description,
        string? externalId,
        Guid? createdByUserId = null,
        CancellationToken cancellationToken = default);

    ValueTask<decimal?> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default);

    ValueTask<Wallet?> GetWalletAsync(Guid walletId, CancellationToken cancellationToken = default);
}
