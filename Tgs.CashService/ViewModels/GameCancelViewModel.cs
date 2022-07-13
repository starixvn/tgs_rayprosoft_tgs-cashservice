namespace Tgs.CashService.ViewModels;

public record GameCancelViewModel
{
    public decimal Amount { get; init; }

    public string ExternalId { get; init; } = default!;
}
