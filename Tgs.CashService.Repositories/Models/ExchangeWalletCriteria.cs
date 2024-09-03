using Tgs.CashService.Models;

namespace Tgs.CashService
{
    public class ExchangeWalletCriteria
    {
        public ExchangeWalletCriteria(ExchangeWalletRecord record, Guid transactionId)
        {
            TransactionId = transactionId.ToString();
            WalletId = record.WalletId;
            PlayerCode = record.PlayerCode;
            TransactionType = (short)record.TransactionType;
            Amount = record.Amount;
            LockAmount = record.LockAmount;
            Description = record.Description;
            ExternalId = record.ExternalId;
            CreatedByUserId = record.UserId;
        }

        public string TransactionId { get; set; }
        public Guid? WalletId { get; set; }
        public string PlayerCode { get; set; }
        public short TransactionType { get; set; }
        public decimal Amount { get; set; }
        public decimal LockAmount { get; set; }
        public string Description { get; set; }
        public string ExternalId { get; set; }
        public Guid? CreatedByUserId { get; set; }
    }
}
