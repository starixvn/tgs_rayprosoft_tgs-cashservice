using System.ComponentModel.DataAnnotations;

namespace Tgs.CashService.ViewModels;

public record GameProviderAdjustBalanceViewModel
{
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal AddedAmount { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal DeductedAmount { get; init; }

    public Guid? WalletId { get; set; }

    public string? Description { get; set; }

    public string? ExternalId { get; set; }
}
