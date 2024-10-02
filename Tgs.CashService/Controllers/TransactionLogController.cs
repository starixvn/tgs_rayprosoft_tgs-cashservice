using Microsoft.AspNetCore.Mvc;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Controllers;

[Route("transaction-log")]
[ApiController]
public class TransactionLogController : ControllerBase
{
    [HttpGet("~/api/transaction-log/{walletId:Guid}", Name = "QueryTransactionLogsOld")]
    [HttpGet("{walletId:Guid}", Name = "QueryTransactionLogs")]
    public async Task<PagingResponseViewModel<TransactionLogViewModel>> QueryTransactionLogsAsync(
        [FromServices] IGetService<TransactionLogQuery, IEnumerable<TransactionLog>> getService,
        [FromQuery] TransactionLogQueryViewModel queryConditions,
        [FromQuery] PagingViewModel pagingViewModel,
        Guid walletId)
    {
        var pageIndex = pagingViewModel.Page ?? 1;
        var query = new TransactionLogQuery(
            walletId,
            (queryConditions.From, queryConditions.To),
            queryConditions.QueryTypes?.Select(n => (TransactionType)n) ?? [],
            pageIndex,
            pagingViewModel.Rows ?? 1000);

        var transactionLogs = await getService.GetAsync(query, HttpContext.RequestAborted).ConfigureAwait(false);

        var items = transactionLogs
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
            Page = pageIndex,
            TotalCount = transactionLogs.FirstOrDefault()?.TotalCount ?? 0
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
    public IAsyncEnumerable<TransactionLogViewModel> QueryInRedisTransactionLogsAsync()
    {
        return AsyncEnumerable.Empty<TransactionLogViewModel>();
    }
}
