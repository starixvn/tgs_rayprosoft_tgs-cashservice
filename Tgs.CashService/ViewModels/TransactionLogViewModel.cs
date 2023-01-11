namespace Tgs.CashService.ViewModels;

public class TransactionLogViewModel
{
    public Guid Id { get; set; }

    public DateTime RecordedOn { get; set; }

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public decimal Balance { get; set; }

    public decimal LockAmount { get; set; }

    public decimal LockedBalance { get; set; }

    public string? Description { get; set; }

    public string? ExternalId { get; set; }
}
