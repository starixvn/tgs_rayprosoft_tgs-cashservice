using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.Paging;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers;

[Route("api/transaction-log")]
[ApiController]
public class TransactionLogController : ControllerBase
{
    [HttpGet("{walletId}", Name = "QueryTransactionLogs")]
    public async Task<PagingResponseViewModel<TransactionLogViewModel>> QueryTransactionLogsAsync(
        [FromServices] IQueryService<TransactionLogQuery, TransactionLog> queryService,
        [FromServices] IGetService<TransactionLogQuery, int> countService,
        [FromQuery] TransactionLogQueryViewModel queryConditions,
        [FromQuery] PagingViewModel pagingViewModel,
        Guid walletId)
    {
        var paging = new Paging<TransactionLog>
        {
            Page = pagingViewModel.Page ?? 1,
            RowCount = pagingViewModel.Rows ?? 1000
        };

        paging.SortByPropertyName(
            pagingViewModel.SortBy ?? nameof(TransactionLog.RecordedOn),
            pagingViewModel.IsDesc ?? true);

        var query = new TransactionLogQuery(
            walletId,
            (queryConditions.From, queryConditions.To),
            queryConditions.QueryTypes?.Select(n => (TransactionType)n) ?? Array.Empty<TransactionType>(),
            paging);

        var totalCount = await countService.GetAsync(
            query,
            HttpContext.RequestAborted).ConfigureAwait(false);

        var items = queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Select(item => new TransactionLogViewModel
            {
                Id = item.Id,
                RecordedOn = item.RecordedOn,
                Amount = item.Amount,
                Balance = item.Balance,
                LockAmount = item.LockAmount,
                LockedBalance = item.LockedBalance,
                Type = item.Type,
                Description = item.Description,
                ExternalId = item.ExternalId
            });

        return new PagingResponseViewModel<TransactionLogViewModel>
        {
            Items = items,
            Page = paging.Page,
            TotalCount = totalCount
        };
    }
}
