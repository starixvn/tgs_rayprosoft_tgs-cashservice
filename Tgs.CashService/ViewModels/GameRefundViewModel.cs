using System.ComponentModel.DataAnnotations;

namespace Tgs.CashService.ViewModels;

public record GameRefundViewModel
{
    [Range(typeof(decimal), minimum: "-79228162514264337593543950335", maximum: "79228162514264337593543950335")]
    public decimal Amount { get; init; }

    public string ExternalId { get; init; } = default!;
}
