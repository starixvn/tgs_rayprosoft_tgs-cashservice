namespace Tgs.CashService
{
    public class ExchangeWalletSPResult
    {
        public string TransactionId { get; set; } = "";
        public decimal Balance { get; set; }
        public decimal LockedBalance { get; set; }
        public int ErrorCode { get; set; }
    }
}
