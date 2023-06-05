using Microsoft.AspNetCore.Mvc;

namespace Tgs.CashQueryService.ViewModels;

public class PagingViewModel
{
    [FromQuery(Name = "skip")]
    public int Skip { get; set; } = 0;

    [FromQuery(Name = "take")]
    public int Take { get; set; } = 10;
}
