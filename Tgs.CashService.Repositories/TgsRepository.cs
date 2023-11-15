using Microsoft.EntityFrameworkCore;
using Tgs.Cash.Common;
using Tgs.CashService.Repositories.Entities;
using Tgs.Paging;

namespace Tgs.CashService.Repositories;

internal class TgsRepository(TgsContext dbContext)
    : ITgsRepository
{
    private readonly TgsContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public IAsyncEnumerable<TransactionLog> QueryTransactionLogsAsync(
        Guid walletId,
        string playerCode,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        Paging<TransactionLog> paging,
        CancellationToken cancellationToken = default)
    {
        var query = MakeTransactionLogQuery(walletId, playerCode, queryRange, queryTypes);

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
                row.RecordType is "T"
                    ? (TransactionType)row.Type
                    : (GameActionType)row.Type switch
                    {
                        GameActionType.PlaceBet => TransactionType.GameBet,
                        GameActionType.WinBet => TransactionType.GameSettleWin,
                        GameActionType.LoseBet => TransactionType.GameSettleLose,
                        GameActionType.FreeBet => TransactionType.GameSettleFree,
                        GameActionType.TieBet => TransactionType.GameSettleTie,
                        GameActionType.CancelTransaction => TransactionType.GameCancel,
                        GameActionType.LockAmount => TransactionType.GameLock,
                        GameActionType.UnlockAmount => TransactionType.GameUnlock,
                        GameActionType.WinJackpot => TransactionType.GameWinJackpot,
                        _ => TransactionType.Unknown
                    },
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

    public async ValueTask<int> CountTransactionLogsQueryAsync(
        Guid walletId,
        string playerCode,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default)
        => await MakeTransactionLogQuery(
            walletId,
            playerCode,
            queryRange,
            queryTypes).CountAsync(cancellationToken).ConfigureAwait(false);

    public async ValueTask<string?> GetPlayerCodeByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var wallet = await _dbContext.Wallets.FindAsync(
            [walletId],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return wallet?.PlayerCode;
    }

    public async ValueTask<decimal> QueryTransactionTotalAmountAsync(
        Guid walletId,
        string playerCode,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes,
        CancellationToken cancellationToken = default)
        => await MakeTransactionLogQuery(
            walletId,
            playerCode,
            queryRange,
            queryTypes)
            .SumAsync(t => t.Amount, cancellationToken)
            .ConfigureAwait(false);

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
            CashType = type.ToString()
        };

        _ = await _dbContext.Wallets.AddAsync(
            wallet,
            cancellationToken).ConfigureAwait(false);

        _ = await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private IQueryable<TransactionData> MakeTransactionLogQuery(
        Guid walletId,
        string playerCode,
        ValueRange<DateTime> queryRange,
        IEnumerable<TransactionType> queryTypes)
    {
        var (from, to) = (queryRange.From.ToUniversalTime(), queryRange.To.ToUniversalTime());

        var transactionQuery = _dbContext.Transactions.AsQueryable()
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

        var gameActionQuery = _dbContext.GameActions.AsQueryable()
            .Where(trx => trx.PlayerCode == playerCode)
            .Where(trx => trx.RecordedOn >= from && trx.RecordedOn < to)
            .Select(trx => new TransactionData
            {
                Id = trx.Id,
                RecordType = "G",
                Amount = trx.Amount,
                LockAmount = trx.Amount,
                Balance = trx.Balance,
                LockedBalance = trx.Balance,
                Type = trx.Type,
                RecordedOn = trx.RecordedOn,
                Description = null,
                ExternalTransactionId = null
            });

        var transactionTypes = queryTypes.Select(t => (short)t).ToArray();
        var gameTypes = queryTypes
            .Select(t => t switch
            {
                TransactionType.GameBet => GameActionType.PlaceBet,
                TransactionType.GameSettleWin => GameActionType.WinBet,
                TransactionType.GameSettleLose => GameActionType.LoseBet,
                TransactionType.GameSettleFree => GameActionType.FreeBet,
                TransactionType.GameSettleTie => GameActionType.TieBet,
                TransactionType.GameCancel => GameActionType.CancelTransaction,
                TransactionType.GameLock => GameActionType.LockAmount,
                TransactionType.GameUnlock => GameActionType.UnlockAmount,
                TransactionType.GameWinJackpot => GameActionType.WinJackpot,
                _ => (GameActionType?)null
            })
            .Where(t => t is not null)
            .Select(t => (short)t!)
            .ToArray();

        if (transactionTypes.Length > 0)
            transactionQuery = transactionQuery.Where(trx => transactionTypes.Contains(trx.Type));
        if (gameTypes.Length > 0)
            gameActionQuery = gameActionQuery.Where(trx => gameTypes.Contains(trx.Type));

        return transactionTypes.Length == 0 || gameTypes.Length > 0
            ? transactionQuery.Concat(gameActionQuery)
            : transactionQuery;
    }
}
