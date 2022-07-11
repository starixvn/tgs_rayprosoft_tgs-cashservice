using System.ComponentModel.DataAnnotations;

namespace Tgs.CashService.ViewModels;

public record GameProviderFundRefundViewModel
{
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal Amount { get; init; }

    public string? Description { get; init; }

    public string? ExternalId { get; init; }
}
