namespace Tgs.CashService.Repositories
{
    public static class StoreProcedureMapper
    {
        public static readonly string ExchangeWalletSP = "dbo.cashservice_exchange_wallet";
    }
    public static class QueryStringMapper
    {
        public static readonly string GetWalletByPlayerCode = @"SELECT w.Id as Id, w.balance as Balance, w.locked_balance as LockedBalance
                                                            FROM dbo.wallets w 
                                                            WHERE w.player_code = @PlayerCode;";
        public static readonly string GetTransactionByTypeAndExternalId = @"SELECT t.id as Id 
                                                                        FROM dbo.transactions t 
                                                                        WHERE t.type = @Type AND t.external_transaction_id = @ExternalId";
        public static readonly string InsertNewWallet = @"INSERT INTO dbo.wallets(
	                                                    id, 
	                                                    player_code, 
	                                                    currency_code, 
	                                                    external_currency_code, 
	                                                    balance, 
                                                        created_on, 
	                                                    updated_on, 
	                                                    locked_balance, 
	                                                    brand_id, 
	                                                    cash_type, 
	                                                    snapshot_id)
	                                                    VALUES (
                                                        @Id, @PlayerCode, @CurrencyCode, @ExternalCurrencyCode, @Balance, @CreatedOn, @UpdatedOn, @LockBalance, @BrandId, @CashType, @SnapshotId)";


    }
}

