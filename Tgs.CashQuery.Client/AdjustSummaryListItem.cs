namespace Tgs.CashQuery.Client;
public record AdjustSummaryListItem(
    Guid BrandId,
    int Company,
    int Agent,
    int System,
    int Transfer,
    int Bonus,
    int AgentOther);
