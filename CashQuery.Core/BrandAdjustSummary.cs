namespace Tgs.CashQueryService;
public record BrandAdjustSummary(
    Guid BrandId,
    int Company,
    int Agent,
    int System,
    int Transfer,
    int Bonus,
    int AgentOther,
    decimal CompanyAmount,
    decimal AgentAmount,
    decimal SystemAmount,
    decimal TransferAmount,
    decimal BonusAmount,
    decimal AgentOtherAmount);
