namespace Tgs.CashService;

public record ExchangeResult(
    Guid TransactionId,
    decimal Balance,
    decimal LockedBalance);
