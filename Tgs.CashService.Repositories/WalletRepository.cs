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
        Guid? createdByUserId,
        CancellationToken cancellationToken = default)
    {
        var updatedOn = DateTime.UtcNow;
        var transactionId = await _serialNumberGenerator
            .GenerateAsync()
            .ConfigureAwait(false);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var connection = _context.Database.GetDbConnection();
        var transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

        var updateResult = await connection.QueryFirstOrDefaultAsync<WalletBalanceReturn>(
            @"
                UPDATE dbo.wallets as C
                SET
                    balance = C.balance + @amount,
                    updated_on = @updateOn
                FROM
                    dbo.wallets AS O
                WHERE
                    C.id = O.id
                    AND
                    C.id = @walletId
                RETURNING
                    O.balance AS OriginBalance,
                    C.balance AS PostBalance",
            new
            {
                walletId,
                amount,
                updatedOn
            },
            transaction).ConfigureAwait(false);

        if (updateResult is null)
            throw new WalletExchangeFailException();

        _ = await _context.Transactions
            .AddAsync(new()
            {
                Id = transactionId,
                PlayerCode = playerCode,
                WalletId = walletId,
                BrandId = brandId,
                Amount = amount,
                Balance = updateResult.PostBalance,
                Type = (short)transactionType,
                Description = description,
                RecordedOn = updatedOn,
                ExternalTransactionId = externalId,
                CreatedByUserId = createdByUserId
            })
            .ConfigureAwait(false);

        _ = await _context.SaveChangesAsync().ConfigureAwait(false);

        scope.Complete();

        return new ExchangeResult(transactionId, updateResult.PostBalance);
    }

    public async ValueTask<decimal?> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var playerWallet = await _context.WalletItems
            .Where(x => x.Id == walletId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return playerWallet?.Balance;
    }

    public async ValueTask<Wallet?> GetWalletAsync(Guid walletId, CancellationToken cancellationToken = default)
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
