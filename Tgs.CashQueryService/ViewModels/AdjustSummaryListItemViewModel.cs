namespace Tgs.CashQueryService.ViewModels;

public class AdjustSummaryListItemViewModel
{
    public Guid BrandId { get; set; }

    public int Company { get; set; }

    public int Agent { get; set; }

    public int System { get; set; }

    public int Transfer { get; set; }

    public int Bonus { get; set; }

    public int AgentOther { get; set; }
}
