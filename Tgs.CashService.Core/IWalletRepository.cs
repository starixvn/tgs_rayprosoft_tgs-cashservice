using System;
using System.Threading.Tasks;

namespace Tgs.CashService.Core
{
    public interface IWalletRepository
    {
        ValueTask<(Guid TransactionId, decimal PostBalance)> ExchangeAsync(
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
}
