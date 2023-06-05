using Microsoft.AspNetCore.Mvc;
using Tgs.CashQueryService.ApplicationServices;
using Tgs.CashQueryService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.Controllers;

[Route("adjust")]
[ApiController]
public class AdjustQueryController : ControllerBase
{
    [HttpGet("player/{brandId}", Name = "QueryPlayerAdjustRecords")]
    public IAsyncEnumerable<AdjustRecordListItemViewModel> QueryPlayerAdjustRecordsAsync(
        [FromServices] IQueryService<AdjustPlayerWalletRecordQuery, AdjustRecord> queryService,
        Guid brandId,
        [FromQuery] AdjustRecordQueryViewModel viewModel,
        [FromQuery] PagingViewModel pagingViewModel)
    {
        var query = new AdjustPlayerWalletRecordQuery(
            brandId,
            (viewModel.From, viewModel.To),
            viewModel.AdjustTypes,
            viewModel.UserId,
            viewModel.Operator,
            pagingViewModel.Skip,
            pagingViewModel.Take);

        return queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Select(record => new AdjustRecordListItemViewModel
            {
                Id = record.Id,
                AdjustType = record.AdjustType,
                UserId = record.UserId,
                Amount = record.Amount,
                BrandId = record.BrandId,
                BucketAmount = record.BucketAmount,
                CreatedOn = record.CreatedOn,
                Operator = record.Operator,
                Remark = record.Remark
            });
    }
}
