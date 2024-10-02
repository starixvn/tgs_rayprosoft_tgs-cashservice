using Dapper;
using Tgs.Cash.Common;
using Tgs.CashService.Core.Entities;
using Tgs.CashService.Models;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashService.Repositories;

internal class TgsRepository(
    IDbAcess dbAcess,
    ISerialNumberGenerator snGenerator)
    : ITgsRepository
{
    public async ValueTask CreateWalletAsync(
        Guid walletId,
        Guid brandId,
        string userId,
        CashType type,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        var currentDate = DateTimeOffset.UtcNow;
        var param = new
        {
            Id = walletId,
            PlayerCode = userId,
            CurrencyCode = currencyCode,
            ExternalCurrencyCode = currencyCode,
            Balance = 0,
            CreatedOn = currentDate,
            UpdatedOn = currentDate,
            LockedBalance = 0,
            BrandId = brandId,
            CashType = type.ToString(),
            SnapshotId = Guid.Empty
        };
        string sqlQuery = $"CALL {StoreProcedureMapper.InsertWalletSP}({string.Join(", ", param.GetType().GetProperties().Select(p => $"{p.Name.ToLower()} => @{p.Name.ToLower()}"))})";
        await dbAcess.ExecuteAsync(sqlQuery, param).ConfigureAwait(false);
    }

    public async ValueTask<IEnumerable<TransactionLog>> QueryTransactionLogsAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var parameters = new
        {
            walletId,
            fromTime = queryRange.From,
            toTime = queryRange.To,
            type = queryTypes.Select(t => (short)t).ToArray(),
            pageSize,
            pageIndex
        };
        var data = await dbAcess.QueryAsync<TransactionData>(FunctionMapper.GetTransactionLogsFC, parameters).ConfigureAwait(false);
        return data.Select(log =>
                            new TransactionLog(
                                log.Id, 
                                log.RecordedOn, 
                                (TransactionType)log.Type, 
                                log.Amount, 
                                log.Balance, 
                                log.LockAmount, 
                                log.LockedBalance, 
                                log.Description,
                                log.ExternalTransactionId, 
                                log.TotalCount
                            ));
    }

    public async ValueTask<decimal> QueryTransactionTotalAmountAsync(
        Guid walletId,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default)
    {
        var parameters = new 
        {
            walletId,
            fromTime = queryRange.From,
            toTime = queryRange.To,
            type = queryTypes.Select(t => (short)t).ToArray(),
        };
        var data = await dbAcess.QueryFirstOrDefaultAsync<dynamic>(FunctionMapper.GetTransactionTotalAmountFC, parameters).ConfigureAwait(false);
        return data?.TotalAmount ?? 0;
    }

    public async ValueTask<string?> GetTransactionIdByProviderInfoAsync(string playerCode, ProviderInfo providerInfo, CancellationToken cancellationToken = default)
    {
        var parameters = new
        {
            playerCode,
            externalTransactionId = providerInfo.RefId,
            providerCode = providerInfo.ProviderCode,
            action = providerInfo.Action
        };
        return (await dbAcess.QueryFirstOrDefaultAsync<Guid?>(FunctionMapper.GetTransactionIdByProviderInfoFC, parameters).ConfigureAwait(false))?.ToString();
    }

    public async ValueTask<Guid> GetWalletIdAsync(string playerCode, Guid? walletIdDefault = default, CancellationToken cancellationToken = default)
    {
        if (walletIdDefault == null || walletIdDefault == Guid.Empty || walletIdDefault == default)
        {
            var wallet = await GetWalletByPlayerCodeAsync(playerCode, cancellationToken).ConfigureAwait(false);
            return wallet is null ? Guid.Empty : wallet.Id;
        }
        return walletIdDefault.GetValueOrDefault();
    }

    public async ValueTask<Wallet?> GetWalletByIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await dbAcess.QueryFirstOrDefaultAsync<Wallet>(FunctionMapper.GetWalletByIdFC, new { id = walletId }).ConfigureAwait(false);
    }

    public async ValueTask<Wallet?> GetWalletByPlayerCodeAsync(string playerCode, CancellationToken cancellationToken = default)
    {
        return await dbAcess.QueryFirstOrDefaultAsync<Wallet>(FunctionMapper.GetWalletByPlayerCodeFC, new { playerCode }).ConfigureAwait(false);
    }

    public async ValueTask<ExchangeResult> ExchangeWalletAsync(ExchangeWalletRecord record, CancellationToken cancellationToken = default)
    {
        return await ExchangeWalletAsync(record, StoreProcedureMapper.ExchangeWalletSP, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<ExchangeResult> ExchangeWalletByActionAsync(ExchangeWalletRecord record, CancellationToken cancellationToken = default)
    {
        return await ExchangeWalletAsync(record, StoreProcedureMapper.ExchangeWalletByActionSP, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<ExchangeResult> ExchangeWalletAsync(ExchangeWalletRecord record, string spName, CancellationToken cancellationToken = default)
    {
        var transactionId = await snGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);
        var criteria = new ExchangeWalletCriteria(record, transactionId);

        var parameters = new DynamicParameters();
        foreach (var property in typeof(ExchangeWalletCriteria).GetProperties())
        {
            var value = property.GetValue(criteria);
            parameters.Add(property.Name.ToLowerInvariant(), value);
        }

        var sqlQuery = $"CALL {spName}({string.Join(", ", typeof(ExchangeWalletCriteria)
            .GetProperties()
            .Select(p => $"{p.Name.ToLowerInvariant()} => @{p.Name.ToLowerInvariant()}"))})";
        var result = await dbAcess.QueryFirstOrDefaultAsync<ExchangeWalletSPResult>(sqlQuery, parameters).ConfigureAwait(false);

        if (result!.ErrorCode == ErrorCode.NotFoundWallet)
        {
            throw new NotFoundWalletException(ErrorMessageMapper.Messages[ErrorCode.NotFoundWallet]);
        }
        if (result!.ErrorCode == ErrorCode.InsufficientFunds)
        {
            throw new InsufficientFundException(ErrorMessageMapper.Messages[ErrorCode.InsufficientFunds]);
        }
        if (result!.ErrorCode == ErrorCode.DuplicateTransaction)
        {
            throw new DuplicateTransactionException(ErrorMessageMapper.Messages[ErrorCode.DuplicateTransaction]);
        }
        if (result!.ErrorCode == ErrorCode.DuplicateTransactionByAction)
        {
            throw new DuplicateTransactionByActionException(ErrorMessageMapper.Messages[ErrorCode.DuplicateTransactionByAction]);
        }

        return new ExchangeResult(Guid.Parse(result.TransactionId), result.Balance, result.LockedBalance);
    }
}
