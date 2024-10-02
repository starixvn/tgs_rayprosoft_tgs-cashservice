namespace Tgs.CashService.Repositories
{
    public static class StoreProcedureMapper
    {
        public static readonly string ExchangeWalletSP = "dbo.cashservice_exchange_wallet";
        public static readonly string ExchangeWalletByActionSP = "dbo.cashservice_exchange_wallet_by_action";
        public static readonly string InsertWalletSP = "dbo.cashservice_wallets_insert";
    }
    public static class FunctionMapper
    {
        public static readonly string GetWalletByPlayerCodeFC = @"SELECT * FROM dbo.wallet_get_balance_by_player_code(player_code => @playerCode)";
        public static readonly string GetWalletByIdFC = @"SELECT * FROM dbo.wallet_get_balance_by_wallet_id(id => @id)";
        public static readonly string GetTransactionIdByProviderInfoFC = @"SELECT * FROM dbo.transaction_get_id_by_action(
                                                                    player_code => @playerCode,
                                                                    external_transaction_id => @externalTransactionId,
                                                                    provider_code => @providerCode,
                                                                    action => @action)";
        public static readonly string GetTransactionLogsFC = @"SELECT * FROM dbo.transaction_get_info(
                                                                    wallet_id => @walletId,
                                                                    from_time => @fromTime,
                                                                    to_time => @toTime,
                                                                    type => @type,
                                                                    page_size => @pageSize,
                                                                    page_index => @pageIndex)";

        public static readonly string GetTransactionTotalAmountFC = @"SELECT * FROM dbo.transaction_get_total_amount(
                                                                    wallet_id => @walletId,
                                                                    from_time => @fromTime,
                                                                    to_time => @toTime,
                                                                    type => @type)";
    }
}

