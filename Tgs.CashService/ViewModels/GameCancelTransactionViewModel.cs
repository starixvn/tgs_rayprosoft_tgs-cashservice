namespace Tgs.CashService.ViewModels;

public record GameCancelTransactionViewModel
{
    public decimal Amount { get; init; }

    public string ExternalId { get; init; } = default!;
}
