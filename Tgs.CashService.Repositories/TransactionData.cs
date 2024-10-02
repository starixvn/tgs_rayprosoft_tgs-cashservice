namespace Tgs.CashService.Repositories;

public class TransactionData
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public decimal LockAmount { get; set; }

    public decimal Balance { get; set; }

    public decimal LockedBalance { get; set; }

    public short Type { get; set; }

    public DateTime RecordedOn { get; set; }

    public string? Description { get; set; }

    public string? ExternalTransactionId { get; set; }

    public int TotalCount { get; set; }
}
