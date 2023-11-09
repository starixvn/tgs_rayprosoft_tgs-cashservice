using System.Text.Json.Serialization;

namespace Tgs.CashService.ViewModels;

public class NewCashReqeustViewModel
{
    public required string UserId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CashType Type { get; set; }

    public required Guid BrandId { get; set; }

    public required string CurrencyCode { get; set; }
}
