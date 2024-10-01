using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tgs.CashService.ViewModels;

public class NewCashRequestViewModel
{
    [Required]
    public string UserId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required]
    public CashType Type { get; set; }

    [Required]
    public Guid BrandId { get; set; }

    [Required]
    public string CurrencyCode { get; set; }
}
