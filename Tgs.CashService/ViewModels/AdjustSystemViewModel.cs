namespace Tgs.CashService.ViewModels;

public class AdjustSystemViewModel
{
    public decimal Amount { get; set; }

    public decimal? LockAmount { get; set; }

    public string? Description { get; set; }

    public string? ExternalId { get; set; }

    public Guid CreatedByUserId { get; set; }
}
