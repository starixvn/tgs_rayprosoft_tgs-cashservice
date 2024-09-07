using Tgs.CashService.Models;

namespace Tgs.CashService
{
    public class ExchangeWalletCriteria
    {
        public ExchangeWalletCriteria(ExchangeWalletRecord record, Guid transactionId)
        {
            TransactionId = transactionId.ToString();
            WalletId = record.WalletId?.ToString();
            PlayerCode = record.PlayerCode;
            TransactionType = (short)record.TransactionType;
            Amount = record.Amount;
            LockAmount = record.LockAmount;
            Description = record.Description;
            ExternalId = record.ExternalId;
            CreatedByUserId = record.UserId?.ToString();
            IsAllowNegative = record.IsAllowNegative;
            ProviderCode = record.ProviderCode;
            OperationId = record.OperationId;
            Action = record.Action;
        }

        public string TransactionId { get; set; }
        public string? WalletId { get; set; }
        public string PlayerCode { get; set; }
        public short TransactionType { get; set; }
        public decimal Amount { get; set; }
        public decimal LockAmount { get; set; }
        public string Description { get; set; }
        public string ExternalId { get; set; }
        public string? CreatedByUserId { get; set; }
        public bool IsAllowNegative { get; set; }
        public string? ProviderCode { get; set; }
        public string? OperationId { get; set; }
        public string? Action { get; set; }

    }
}
