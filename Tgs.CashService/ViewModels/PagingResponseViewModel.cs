namespace Tgs.CashService.ViewModels;

public class PagingResponseViewModel<TItem>
{
#pragma warning disable AMNF0001 // Awaitable (Asynchronous) methods should be suffixed with 'Async'
    public IAsyncEnumerable<TItem> Items { get; set; } = default!;

#pragma warning restore AMNF0001 // Awaitable (Asynchronous) methods should be suffixed with 'Async'

    public int Page { get; set; }

    public int TotalCount { get; set; }
}
