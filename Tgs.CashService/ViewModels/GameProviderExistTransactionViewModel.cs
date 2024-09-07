namespace Tgs.CashService.ViewModels;

public record GameProviderExistTransactionViewModel
{
    public string? ExternalId { get; set; }
    public string? ProviderCode { get; set; }
    public string? Action { get; set; }
}
