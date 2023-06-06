namespace Tgs.CashQueryService;
public record BrandAdjustSummary(
    Guid BrandId,
    int Company,
    int Agent,
    int System,
    int Transfer,
    int Bonus);
