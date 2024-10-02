using Tgs.Cash.Common;
using Tgs.CashService.Core.Entities;
using Tgs.CashService.Models;

namespace Tgs.CashService;

public interface ITgsRepository
{
    ValueTask CreateWalletAsync(
        Guid walletId,
        Guid brandId,
        string userId,
        CashType type,
        string currencyCode,
        CancellationToken cancellationToken = default);

    ValueTask<IEnumerable<TransactionLog>> QueryTransactionLogsAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    ValueTask<decimal> QueryTransactionTotalAmountAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default);

    ValueTask<string?> GetTransactionIdByProviderInfoAsync(string playerCode, ProviderInfo providerInfo, CancellationToken cancellationToken = default);

    ValueTask<Guid> GetWalletIdAsync(
        string playerCode,
        Guid? walletIdDefault = default,
        CancellationToken cancellationToken = default);

    ValueTask<Wallet?> GetWalletByIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    ValueTask<Wallet?> GetWalletByPlayerCodeAsync(string playerCode, CancellationToken cancellationToken = default);

    ValueTask<ExchangeResult> ExchangeWalletAsync(ExchangeWalletRecord record, CancellationToken cancellationToken = default);

    ValueTask<ExchangeResult> ExchangeWalletByActionAsync(ExchangeWalletRecord record, CancellationToken cancellationToken = default);
}
