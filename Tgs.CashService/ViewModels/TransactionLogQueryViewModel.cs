using Microsoft.AspNetCore.Mvc;

namespace Tgs.CashService.ViewModels;

public class TransactionLogQueryViewModel
{
    [FromQuery(Name = "from")]
    public DateTime From { get; set; }

    [FromQuery(Name = "to")]
    public DateTime To { get; set; }

    [FromQuery(Name = "type")]
    public IEnumerable<int>? QueryTypes { get; set; }
}
