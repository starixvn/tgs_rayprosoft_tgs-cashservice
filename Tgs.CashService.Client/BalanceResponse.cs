namespace Tgs.CashService.Client;

public record BalanceResponse
{
    public Guid TransactionId { get; init; }

    public decimal Balance { get; init; }

    public decimal LockedBalance { get; init; }

    public int ErrorCode { get; init; }

    public string ErrorMessage { get; init; } = "";
}
