using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Tgs.Cash.Common;
using Tgs.CashService.Repositories.Entities;
using Tgs.CashService.Core.Entities;
using Tgs.Paging;
using Microsoft.Extensions.Logging;
using Tgs.SerialNumberGenerator;
using Tgs.CashService.Models;
using Dapper;

namespace Tgs.CashService.Repositories;

internal class TgsRepository(
    TgsContext dbContext,
    IDbAcess dbAcess,
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

    public int CountInRedisTransactionLogs(Guid walletId, CancellationToken cancellationToken = default)
    {
        return 0;
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
        var param = new
        {
            Id = walletId,
            PlayerCode = userId,
            CurrencyCode = currencyCode,
            ExternalCurrencyCode = currencyCode,
            Balance = 0,
            CreatedOn = currentDate,
            UpdatedOn = currentDate,
            LockBalance = 0,
            BrandId = brandId,
            CashType = type.ToString(),
            SnapshotId = Guid.Empty
        };
        await dbAcess.ExecuteAsync(QueryStringMapper.InsertNewWallet, param).ConfigureAwait(false);
    }

    public IAsyncEnumerable<TransactionLog> QueryInRedisTransactionLogsAsync(
        Guid walletId,
        CancellationToken cancellationToken = default)
        => AsyncEnumerable.Empty<TransactionLog>();

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

        var transactionQuery = dbContext.Transactions.AsNoTracking().AsQueryable()
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

    public async ValueTask<Transaction?> GetTransactionLogByProviderInfoAsync(string playerCode, ProviderInfo providerInfo, CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("PlayerCode", playerCode);
        parameters.Add("ExternalId", providerInfo.RefId);
        parameters.Add("ProviderCode", providerInfo.ProviderCode);
        parameters.Add("Action", providerInfo.Action);
        return await dbAcess.QueryFirstOrDefaultAsync<Transaction>(QueryStringMapper.GetTransactionByAction, parameters).ConfigureAwait(false);
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
            .AsNoTracking()
            .FirstOrDefaultAsync(wallet => wallet.Id == walletId)
            .ConfigureAwait(false);
    }

    public async ValueTask<Wallet?> GetWalletByPlayerCodeAsync(string playerCode, CancellationToken cancellationToken = default)
    {
        var parameters = new {PlayerCode = playerCode};
        return await dbAcess.QueryFirstOrDefaultAsync<Wallet>(QueryStringMapper.GetWalletByPlayerCode, parameters).ConfigureAwait(false);
    }

    public async ValueTask<ExchangeResult> ExchangeWalletAsync(ExchangeWalletRecord record, CancellationToken cancellationToken = default)
    {
        var transactionId = await snGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);
        var criteria = new ExchangeWalletCriteria(record, transactionId);
        var spName = StoreProcedureMapper.ExchangeWalletSP;

        var parameters = new DynamicParameters();
        foreach (var property in typeof(ExchangeWalletCriteria).GetProperties()) 
        { 
            var value = property.GetValue(criteria);
            parameters.Add(property.Name.ToLowerInvariant(), value);
        }

        var sqlQuery = $"CALL {spName}({string.Join(", ", typeof(ExchangeWalletCriteria).GetProperties().Select(p => $"@{p.Name.ToLowerInvariant()}"))})";
        var result = await dbAcess.QueryFirstOrDefaultAsync<ExchangeWalletSPResult>(sqlQuery, parameters).ConfigureAwait(false);

        if (result!.ErrorCode == ErrorCode.NotFoundWallet)
        {
            logger.LogError($"Can't found wallet {record.WalletId}.");
            throw new NotFoundWalletException(ErrorMessages.Messages[ErrorCode.NotFoundWallet]);
        }
        if (result!.ErrorCode == ErrorCode.InsufficientFunds)
        {
            logger.LogError($"Invalid Balance {JsonSerializer.Serialize(record)}");
            throw new InsufficientFundException(ErrorMessages.Messages[ErrorCode.InsufficientFunds]);
        }
        if(result!.ErrorCode == ErrorCode.DuplicateTransaction)
        {
            logger.LogError($"Duplicate Transaction {JsonSerializer.Serialize(record)}");
            throw new DuplicateTransactionException(ErrorMessages.Messages[ErrorCode.DuplicateTransaction]);
        }

        return new ExchangeResult(Guid.Parse(result.TransactionId), result.Balance, result.LockedBalance);
    }
}
