namespace Tgs.CashService.Client;
public record PagingResponse<TItem>(
    IEnumerable<TItem> Items,
    int Page,
    int TotalCount);
