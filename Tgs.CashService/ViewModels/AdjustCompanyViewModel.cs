namespace Tgs.CashService.ViewModels;

public record AdjustCompanyViewModel
{
    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string? ExternalId { get; set; }

    public Guid CreatedByUserId { get; set; }
}
