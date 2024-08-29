using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tgs.Cash.Common;
using Tgs.CashService.Repositories.Entities;
using Tgs.CashService.Core.Entities;
using Tgs.Paging;
using Microsoft.Extensions.Logging;
using Tgs.Cash.Protos;
using Tgs.SerialNumberGenerator;
using Tgs.CashService.Models;
using System.Threading;

namespace Tgs.CashService.Repositories;

internal class TgsRepository(
    TgsContext dbContext,
    IDatabase redisDatabase,
    ISerialNumberGenerator snGenerator,
    ILogger<TgsRepository> logger)
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
        dbContext.Wallets.Add(wallet);

        var transactionId = await snGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);
        var transaction = new Transaction
        {
            Id = transactionId,
            BrandId = wallet.BrandId!.Value,
            PlayerCode = wallet.PlayerCode,
            WalletId = walletId,
            Type = (short)TransactionType.CreateWallet,
            Amount = 0,
            LockAmount = 0,
            Balance = 0,
            LockedBalance = 0,
            Description = "Create Wallet",
            ExternalTransactionId = string.Empty,
            CreatedByUserId = null,
            RecordedOn = DateTime.UtcNow.ToUniversalTime(),
            SentOn = null
        };
        dbContext.Transactions.Add(transaction);

        _ = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<string?> GetPlayerCodeByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var wallet = await dbContext.Wallets
            .FindAsync(
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

    public async ValueTask<Guid> GetWalletIdAsync(string playerCode,Guid? walletIdDefault = default, CancellationToken cancellationToken = default)
    {
        if (walletIdDefault == null ||walletIdDefault == Guid.Empty || walletIdDefault == default)
        {
            var wallet = await GetWalletByPlayerCodeAsync(playerCode).ConfigureAwait(false);
            return wallet is null ? Guid.Empty : wallet.Id;
        }
        return walletIdDefault.GetValueOrDefault();
    }
    public async ValueTask<Wallet?> GetWalletByIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Wallets
            .FirstOrDefaultAsync(wallet => wallet.Id == walletId)
            .ConfigureAwait(false);
    }

    public async ValueTask<Wallet?> GetWalletByPlayerCodeAsync(string playerCode, CancellationToken cancellationToken = default)
    {
        var wallet = await dbContext.Wallets
            .FirstOrDefaultAsync(w => w.PlayerCode == playerCode)
            .ConfigureAwait(false);
        return wallet;
    }

    public async ValueTask<ExchangeResult> ExchangeWalletAsync(ExchangeWalletRecord record, CancellationToken cancellationToken = default)
    {
        var wallet = await dbContext.Wallets
            .SingleAsync(wallet => wallet.Id == record.WalletId || wallet.PlayerCode == record.PlayerCode)
            .ConfigureAwait(false);
        if (wallet == null)
        {
            logger.LogError($"Can't found wallet {record.WalletId}.");
            throw new InvalidOperationException("Can't found wallet.");
        }
        var transactionId = await snGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);
        var transaction = new Transaction
        {
            Id = transactionId,
            BrandId = wallet.BrandId!.Value,
            PlayerCode = wallet.PlayerCode,
            WalletId = wallet.Id,
            Type = (short)record.TransactionType,
            Amount = record.Amount,
            LockAmount = record.LockAmount,
            Balance = wallet.Balance + record.Amount,
            LockedBalance = wallet.LockedBalance + record.LockAmount,
            Description = record.Description,
            ExternalTransactionId = record.ExternalId,
            CreatedByUserId = record.UserId,
            RecordedOn = DateTime.UtcNow.ToUniversalTime(),
            SentOn = null
        };
        if (transaction.LockedBalance < 0M || transaction.Balance < 0M)
        {
            logger.LogError($"Invalid Balance {JsonSerializer.Serialize(record)}");
            throw new InvalidOperationException();
        }
        await dbContext.Transactions.AddAsync(transaction).ConfigureAwait(false);

        wallet.Balance = transaction.Balance;
        wallet.LockedBalance = transaction.LockedBalance;
        wallet.SnapshotId = transactionId;
        wallet.UpdatedOn = transaction.RecordedOn;

        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        return new ExchangeResult(transactionId, transaction.Balance, transaction.LockedBalance);
    }
}
