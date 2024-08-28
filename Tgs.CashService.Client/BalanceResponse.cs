namespace Tgs.CashService.Client;

public record BalanceResponse
{
    public Guid TransactionId { get; init; }

    public decimal Balance { get; init; }

    public decimal LockedBalance { get; init; }

    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; } = "";
}
