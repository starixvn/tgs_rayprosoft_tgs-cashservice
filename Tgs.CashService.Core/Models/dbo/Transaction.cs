namespace Tgs.CashService.Core.Entities;

public partial class Transaction
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public decimal Balance { get; set; }

    public Guid BrandId { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public string? Description { get; set; }

    public string? ExternalTransactionId { get; set; }

    public decimal LockAmount { get; set; }

    public decimal LockedBalance { get; set; }

    public string PlayerCode { get; set; } = "";

    public DateTime RecordedOn { get; set; }

    public DateTime? SentOn { get; set; }

    public short Type { get; set; }

    public Guid WalletId { get; set; }

    public string ProviderCode { get; set; }
    public string OperationId { get; set; }
    public string Action { get; set; }
}