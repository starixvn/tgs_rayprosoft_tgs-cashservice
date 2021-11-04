using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tgs.CashService.Core;
using TGS.GameProviders.Repositories.Entities;

namespace Tgs.CashService.Repositories
{
    internal class WalletRepository : IWalletRepository
    {
        private readonly TgsContext _context;

        public WalletRepository(TgsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async ValueTask<(Guid TransactionId, decimal PostBalance)> ExchangeAsync(
            Guid brandId,
            Guid walletId,
            string playerCode,
            TransactionTypes transactionType,
            decimal amount,
            string? description,
            string? externalId,
            Guid? createdByUserId)
        {
            const string sql = "dbo.wallet_update_wallet_balance";
            var updatedOn = DateTimeOffset.UtcNow;
            var transactionId = Guid.NewGuid();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var connection = _context.Database.GetDbConnection();
            var transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

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
                transaction: transaction);

            await _context.Transactions
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
                    RecordedOn = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+8)).DateTime,
                    ExternalTransactionId = externalId,
                    CreatedByUserId = createdByUserId
                })
                .ConfigureAwait(false);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            scope.Complete();

            return (transactionId, postBalance);
        }

        public async ValueTask<decimal?> GetBalanceAsync(Guid walletId)
        {
            var playerWallet = await _context.Wallets
                .Where(x => x.Id == walletId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return playerWallet?.Balance;
        }
    }
}
