using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.Cash.Common;
using Tgs.CashService.Repositories.Entities;
using Tgs.Paging;

namespace Tgs.CashService.Repositories;

internal class TgsRepository(
    TgsContext dbContext,
    IDatabase redisDatabase)
    : ITgsRepository
{
    private static readonly JsonSerializerOptions _SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = false,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public async ValueTask<int> CountInRedisTransactionLogsAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var length = await redisDatabase.ListLengthAsync(
            $"Tgs:Transactions:{walletId}").ConfigureAwait(false);

        return (int)length;
    }

    public async ValueTask<int> CountTransactionLogsQueryAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default)
        => await MakeTransactionLogQuery(
            walletId,
            queryRange,
            queryTypes).CountAsync(cancellationToken).ConfigureAwait(false);

    public async ValueTask CreateWalletAsync(
        Guid walletId,
        Guid brandId,
        string userId,
        CashType type,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        var currentDate = DateTime.UtcNow;

        var wallet = new Wallet
        {
            Id = walletId,
            PlayerCode = userId,
            CurrencyCode = currencyCode,
            ExternalCurrencyCode = currencyCode,
            Balance = 0,
            CreatedOn = currentDate,
            UpdatedOn = currentDate,
            BrandId = brandId,
            CashType = type.ToString(),
            SnapshotId = Guid.Empty
        };

        _ = await dbContext.Wallets.AddAsync(
            wallet,
            cancellationToken).ConfigureAwait(false);

        _ = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<string?> GetPlayerCodeByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var wallet = await dbContext.Wallets.FindAsync(
            [walletId],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return wallet?.PlayerCode;
    }

    public IAsyncEnumerable<TransactionLog> QueryInRedisTransactionLogsAsync(
        Guid walletId,
        CancellationToken cancellationToken = default)
        => redisDatabase.ListRangeAsync($"Tgs:Transactions:{walletId}")
            .ToAsyncEnumerable()
            .SelectMany(values => values.ToAsyncEnumerable())
            .Select(value => JsonSerializer.Deserialize<TransactionRecord>((string)value!, _SerializerOptions))
            .Where(record => record is not null)
            .Select(record => record!)
            .Select(record => new TransactionLog(
                record.Id,
                record.RecordedOn,
                record.Type,
                record.Amount,
                record.Balance,
                record.LockAmount,
                record.LockedBalance,
                record.Description,
                record.ExternalId))
            .ToArrayAsync(cancellationToken)
            .AsTask()
            .ToAsyncEnumerable()
            .SelectMany(arr => arr.ToAsyncEnumerable());

    public IAsyncEnumerable<TransactionLog> QueryTransactionLogsAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        Paging<TransactionLog> paging,
        CancellationToken cancellationToken = default)
    {
        var query = MakeTransactionLogQuery(walletId, queryRange, queryTypes);

        var dbPaging = paging.ConvertTo<TransactionData>(newPaging =>
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
                row.RecordType is "G"
                    ? 0M
                    : row.LockAmount,
                row.RecordType is "G"
                    ? 0M
                    : row.LockedBalance,
                row.Description,
                row.ExternalTransactionId));
    }

    public async ValueTask<decimal> QueryTransactionTotalAmountAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default)
        => await MakeTransactionLogQuery(
            walletId,
            queryRange,
            queryTypes)
            .SumAsync(t => t.Amount, cancellationToken)
            .ConfigureAwait(false);

    private IQueryable<TransactionData> MakeTransactionLogQuery(
            Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes)
    {
        var (from, to) = (queryRange.From.ToUniversalTime(), queryRange.To.ToUniversalTime());

        var transactionQuery = dbContext.Transactions.AsQueryable()
            .Where(trx => trx.WalletId == walletId)
            .Where(trx => trx.RecordedOn >= from && trx.RecordedOn < to)
            .Select(trx => new TransactionData
            {
                Id = trx.Id,
                RecordType = "T",
                Amount = trx.Amount,
                LockAmount = trx.LockAmount,
                Balance = trx.Balance,
                LockedBalance = trx.LockedBalance,
                Type = trx.Type,
                RecordedOn = trx.RecordedOn,
                Description = trx.Description,
                ExternalTransactionId = trx.ExternalTransactionId
            });

        var transactionTypes = queryTypes.Select(t => (short)t).ToArray();

        if (transactionTypes.Length > 0)
            transactionQuery = transactionQuery.Where(trx => transactionTypes.Contains(trx.Type));

        return transactionQuery;
    }
}
