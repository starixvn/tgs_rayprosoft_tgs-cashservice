using System.Data;
using System.Transactions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tgs.CashServiceWorker;
using Tgs.SerialNumberGenerator;
using TGS.GameProviders.Repositories.Entities;

namespace Tgs.CashService.Repositories;

internal class WalletRepository : IWalletRepository
{
    private readonly TgsContext _context;
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    public WalletRepository(TgsContext context, ISerialNumberGenerator serialNumberGenerator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
    }

    public async ValueTask<ExchangeResult> ExchangeAsync(
        Guid brandId,
        Guid walletId,
        string playerCode,
        TransactionType transactionType,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createdByUserId)
    {
        const string sql = "dbo.wallet_update_wallet_balance";
        var updatedOn = DateTime.UtcNow;
        var transactionId = await _serialNumberGenerator
            .GenerateAsync()
            .ConfigureAwait(false);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var connection = _context.Database.GetDbConnection();
        var transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

        await connection.QueryAsync(
            @"
                UPDATE dbo.wallets as C
                SET
                    balance = C.balance + :amount,
                    updated_on = :updateOn
                FROM
                    dbo.wallets AS O
                WHERE
                    C.id = O.id
                    AND
                    C.id = :walletId
                RETURNING 
                    O.balance AS OriginBalance, 
                    C.balance AS Balance",
            new
            {
                walletId,
                amount,
                updatedOn
            }).ConfigureAwait(false);

        var postBalance = await connection.QueryFirstOrDefaultAsync<decimal>(
            sql,
            new Dictionary<string, object>
            {
                    {":player_code", playerCode},
                    {":wallet_id", walletId},
                    {":amount", amount},
                    {":updated_on", updatedOn}
            },
            commandType: CommandType.StoredProcedure,
            transaction: transaction).ConfigureAwait(false);

        _ = await _context.Transactions
            .AddAsync(new()
            {
                Id = transactionId,
                PlayerCode = playerCode,
                WalletId = walletId,
                BrandId = brandId,
                Amount = amount,
                Balance = postBalance,
                Type = (short)transactionType,
                Description = description,
                RecordedOn = updatedOn,
                ExternalTransactionId = externalId,
                CreatedByUserId = createdByUserId
            })
            .ConfigureAwait(false);

        _ = await _context.SaveChangesAsync().ConfigureAwait(false);

        scope.Complete();

        return new ExchangeResult(transactionId, postBalance);
    }

    public async ValueTask<decimal?> GetBalanceAsync(Guid walletId)
    {
        var playerWallet = await _context.WalletItems
            .Where(x => x.Id == walletId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return playerWallet?.Balance;
    }

    public async ValueTask<Wallet?> GetWalletAsync(Guid walletId)
    {
        var wallet = await (from w in _context.WalletItems.AsQueryable()
                            join p in _context.Players on w.PlayerCode equals p.PlayerCode
                            where w.Id == walletId
                            select new Wallet
                            {
                                Id = w.Id,
                                PlayerCode = w.PlayerCode,
                                BrandId = p.BrandId
                            }).FirstOrDefaultAsync().ConfigureAwait(false);

        return wallet;
    }
}
