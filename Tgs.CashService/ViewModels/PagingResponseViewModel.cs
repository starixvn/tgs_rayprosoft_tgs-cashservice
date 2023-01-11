namespace Tgs.CashService.ViewModels;

public class PagingResponseViewModel<TItem>
{
    public IAsyncEnumerable<TItem> Items { get; set; } = default!;

    public int Page { get; set; }

    public int TotalCount { get; set; }
}
