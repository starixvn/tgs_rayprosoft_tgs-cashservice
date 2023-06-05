using Microsoft.EntityFrameworkCore;
using Tgs.Cash.Common;
using Tgs.CashService.Repositories.Entities;
using Tgs.Paging;

namespace Tgs.CashService.Repositories;

internal class TgsRepository : ITgsRepository
{
    private readonly TgsContext _dbContext;

    public TgsRepository(TgsContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<TransactionLog> QueryTransactionLogsAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        Paging<TransactionLog> paging,
        CancellationToken cancellationToken = default)
    {
        var query = MakeTransactionLogQuery(walletId, queryRange, queryTypes);

        var dbPaging = paging.ConvertTo<Transaction>(newPaging =>
        {
            _ = newPaging.MapProperty(
                nameof(TransactionLog.RecordedOn),
                row => row.RecordedOn);
        });

        query = query.ApplyOrderBy(dbPaging).ApplyPaging(dbPaging);

        return query.ToAsyncEnumerable()
            .Select(row => new TransactionLog(
                row.Id,
                row.RecordedOn,
                (TransactionType)row.Type,
                row.Amount,
                row.Balance,
                row.LockAmount,
                row.LockedBalance,
                row.Description,
                row.ExternalTransactionId));
    }

    public async ValueTask<int> CountTransactionLogsQueryAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default)
        => await MakeTransactionLogQuery(
            walletId,
            queryRange,
            queryTypes).CountAsync(cancellationToken);

    private IQueryable<Transaction> MakeTransactionLogQuery(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes)
    {
        var (from, to) = (queryRange.From.ToUniversalTime(), queryRange.To.ToUniversalTime());
        var types = queryTypes.Select(t => (short)t).ToArray();

        var query = _dbContext.Transactions.AsQueryable()
            .Where(trx => trx.WalletId == walletId)
            .Where(trx => trx.RecordedOn >= from && trx.RecordedOn < to);

        if (types.Length > 0)
            query = query.Where(trx => types.Contains(trx.Type));

        return query;
    }
}
