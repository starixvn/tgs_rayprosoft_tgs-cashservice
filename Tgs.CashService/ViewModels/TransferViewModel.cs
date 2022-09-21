namespace Tgs.CashService.ViewModels;

public class TransferViewModel
{
    public decimal Amount { get; init; }

    public decimal LockAmount { get; init; }

    public string? Description { get; init; }

    public Guid ExternalId { get; init; }
}
