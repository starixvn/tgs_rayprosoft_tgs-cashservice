using Tgs.CashService;

namespace Tgs.CashServiceWorker;

public interface IWalletRepository
{
    ValueTask<decimal?> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default);

    ValueTask<Wallet?> GetWalletAsync(Guid walletId, CancellationToken cancellationToken = default);

    ValueTask WriteTransactionRecordAsync(TransactionRecord record, CancellationToken cancellationToken = default);
}
