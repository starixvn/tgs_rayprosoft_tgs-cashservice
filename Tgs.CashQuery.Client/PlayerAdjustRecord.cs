namespace Tgs.CashQueryService.Client;

public record PlayerAdjustRecord(
    Guid TransactionId,
    bool IncludeInNetProfit);
