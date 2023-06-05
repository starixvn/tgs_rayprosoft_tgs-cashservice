using Microsoft.AspNetCore.Mvc;

namespace Tgs.CashQueryService.ViewModels;

public class AdjustRecordQueryViewModel
{
    [FromQuery(Name = "from")]
    public required DateTime From { get; set; }

    [FromQuery(Name = "to")]
    public required DateTime To { get; set; }

    [FromQuery(Name = "type")]
    public IEnumerable<AdjustType>? AdjustTypes { get; set; }

    [FromQuery(Name = "userId")]
    public string? UserId { get; set; }

    [FromQuery(Name = "operator")]
    public string? Operator { get; set; }
}
