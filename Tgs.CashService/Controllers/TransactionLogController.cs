using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.Paging;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers;

[Route("transaction-log")]
[ApiController]
public class TransactionLogController : ControllerBase
{
    [HttpGet("~/api/transaction-log/{walletId:Guid}", Name = "QueryTransactionLogsOld")]
    [HttpGet("{walletId:Guid}", Name = "QueryTransactionLogs")]
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
            queryConditions.QueryTypes?.Select(n => (TransactionType)n) ?? [],
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

    [HttpGet("~/api/transaction-log/totalamount/{walletId:Guid}", Name = "QueryTransactionTotalAmountOld")]
    [HttpGet("totalamount/{walletId:Guid}", Name = "QueryTransactionTotalAmount")]
    public ValueTask<decimal> QueryTransactionTotalAmountAsync(
        [FromServices] IGetService<TransactionTotalAmountQuery, decimal> getService,
        [FromQuery] TransactionLogQueryViewModel queryConditions,
        Guid walletId)
    {
        var query = new TransactionTotalAmountQuery(
            walletId,
            (queryConditions.From, queryConditions.To),
            queryConditions.QueryTypes?.Select(n => (TransactionType)n) ?? []);

        return getService.GetAsync(
            query,
            HttpContext.RequestAborted);
    }

    [HttpGet("buffer/{walletId:Guid}", Name = "QueryInRedisTransactionLogs")]
    public IAsyncEnumerable<TransactionLogViewModel> QueryInRedisTransactionLogsAsync(
        [FromServices] IQueryService<InRedisTransactionLogQuery, TransactionLog> queryService,
        [FromServices] IGetService<InRedisTransactionLogQuery, int> countService,
        Guid walletId,
        [FromQuery] PagingViewModel pagingViewModel)
    {
        var paging = new Paging<TransactionLog>
        {
            Page = pagingViewModel.Page ?? 1,
            RowCount = pagingViewModel.Rows ?? 1000
        };

        paging.SortByPropertyName(
            pagingViewModel.SortBy ?? nameof(TransactionLog.RecordedOn),
            pagingViewModel.IsDesc ?? true);

        var query = new InRedisTransactionLogQuery(
            walletId,
            paging);

        return countService.GetAsync(
            query,
            HttpContext.RequestAborted)
            .AsTask()
            .ToAsyncEnumerable()
            .Take(1)
            .Do(totalCount => Response.Headers.Append("TGS-TotalCount", totalCount.ToString()))
            .SelectMany(_ => queryService.QueryAsync(query, HttpContext.RequestAborted)
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
                }));
    }
}
