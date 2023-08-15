namespace Tgs.CashQueryService;

public record PlayerAdjustRecord(
    Guid TransactionId,
    bool IncludeInNetProfit);
