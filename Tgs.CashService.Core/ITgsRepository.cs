using Tgs.Cash.Common;
using Tgs.Cash.Protos;
using Tgs.CashService.Core.Entities;
using Tgs.CashService.Models;
using Tgs.Paging;

namespace Tgs.CashService;

public interface ITgsRepository
{

    int CountInRedisTransactionLogs(Guid walletId, CancellationToken cancellationToken = default);
    ValueTask<int> CountTransactionLogsQueryAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default);

    ValueTask CreateWalletAsync(
        Guid walletId,
        Guid brandId,
        string userId,
        CashType type,
        string currencyCode,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<TransactionLog> QueryInRedisTransactionLogsAsync(
        Guid walletId,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<TransactionLog> QueryTransactionLogsAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        Paging<TransactionLog> paging,
        CancellationToken cancellationToken = default);

    ValueTask<decimal> QueryTransactionTotalAmountAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default);

    ValueTask<Transaction?> GetTransactionLogByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);

    ValueTask<Guid> GetWalletIdAsync(
        string playerCode,
        Guid? walletIdDefault = default,
        CancellationToken cancellationToken = default);

    ValueTask<Wallet?> GetWalletByIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    ValueTask<Wallet?> GetWalletByPlayerCodeAsync(string playerCode, CancellationToken cancellationToken = default);

    ValueTask<ExchangeResult> ExchangeWalletAsync(ExchangeWalletRecord record, CancellationToken cancellationToken = default);
}
