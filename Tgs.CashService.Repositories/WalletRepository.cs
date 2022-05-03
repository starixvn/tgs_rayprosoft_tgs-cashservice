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
    private readonly IDbScriptExecutor _dbScriptExecutor;
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    public WalletRepository(TgsContext context, IDbScriptExecutor dbScriptExecutor, ISerialNumberGenerator serialNumberGenerator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbScriptExecutor = dbScriptExecutor ?? throw new ArgumentNullException(nameof(dbScriptExecutor));
        _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
    }

    public async ValueTask<ExchangeResult> ExchangeAsync(
        Guid brandId,
        Guid walletId,
        string playerCode,
        TransactionType transactionType,
        decimal amount,
        decimal lockAmount,
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

        var updateResult = await _dbScriptExecutor.QueryFirstOrDefaultAsync<WalletBalanceReturn>(
            @"
                UPDATE dbo.wallets AS C
                SET
                    balance = C.balance + @amount,
                    locked_balance = C.locked_balance + @lockAmount,
                    updated_on = @updatedOn
                FROM
                    dbo.wallets AS O
                WHERE
                    C.id = O.id
                    AND
                    C.id = @walletId
                RETURNING
                    O.balance AS OriginBalance,
                    O.locked_balance AS OriginLockedBalance,
                    C.balance AS PostBalance,
                    C.locked_balance AS PostLockedBalance",
            new
            {
                walletId,
                amount,
                lockAmount,
                updatedOn
            }).ConfigureAwait(false);

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
                LockAmount = lockAmount,
                Balance = updateResult.PostBalance,
                LockedBalance = updateResult.PostLockedBalance,
                Type = (short)transactionType,
                Description = description,
                RecordedOn = updatedOn,
                ExternalTransactionId = externalId,
                CreatedByUserId = createdByUserId
            }, cancellationToken)
            .ConfigureAwait(false);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        scope.Complete();

        return new ExchangeResult(
            transactionId,
            updateResult.PostBalance,
            updateResult.PostLockedBalance);
    }

    public async ValueTask<decimal?> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var playerWallet = await _context.WalletItems
            .Where(x => x.Id == walletId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken)
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
                            }).FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return wallet;
    }
}
