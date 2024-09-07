namespace Tgs.CashService.ViewModels;

public record GameProviderAdjustBalanceViewModel
{
    public decimal AddedAmount { get; init; }
    public decimal DeductedAmount { get; init; }
    public string? Description { get; set; }
    public string? ExternalId { get; set; }
    public string? ProviderCode { get; set; }
    public string? OperationId { get; set; }
    public string? Action { get; set; }
}
