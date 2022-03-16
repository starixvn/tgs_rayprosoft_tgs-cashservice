using System.ComponentModel.DataAnnotations;

namespace Tgs.CashService.ViewModels;

public record GameProviderCancelSettlementViewModel
{
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal Amount { get; init; }

    public string ExternalId { get; init; } = default!;
}
