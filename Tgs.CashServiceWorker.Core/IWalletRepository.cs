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
      string? description,
      string? externalId,
      Guid? createdByUserId = null);

    ValueTask<decimal?> GetBalanceAsync(Guid walletId);

    ValueTask<Wallet?> GetWalletAsync(Guid walletId);
}
