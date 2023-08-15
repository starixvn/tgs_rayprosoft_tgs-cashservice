namespace Tgs.CashQueryService.ApplicationServices;

public record AdjustPlayerNetProfitQuery(IEnumerable<Guid> TransactionIds);
