namespace Tgs.CashService.ViewModels;

public record TransactionResultViewModel(
    Guid TransactionId,
    decimal Balance,
    decimal LockedBalance);
