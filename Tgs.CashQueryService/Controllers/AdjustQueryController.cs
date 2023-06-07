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
            pagingViewModel.Take,
            pagingViewModel.Skip);

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

    [HttpGet("agent/{brandId}", Name = "QueryAgentAdjustRecords")]
    public IAsyncEnumerable<AdjustRecordListItemViewModel> QueryAgentAdjustRecordsAsync(
        [FromServices] IQueryService<AdjustAgentWalletRecordQuery, AdjustRecord> queryService,
        Guid brandId,
        [FromQuery] AdjustRecordQueryViewModel viewModel,
        [FromQuery] PagingViewModel pagingViewModel)
    {
        var query = new AdjustAgentWalletRecordQuery(
            brandId,
            (viewModel.From, viewModel.To),
            viewModel.AdjustTypes,
            string.IsNullOrWhiteSpace(viewModel.UserId)
                ? null
                : Guid.Parse(viewModel.UserId),
            viewModel.Operator,
            pagingViewModel.Take,
            pagingViewModel.Skip);

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

    [HttpGet("summary/player", Name = "QueryPlayerAdjustSummaries")]
    public IAsyncEnumerable<AdjustSummaryListItemViewModel> QueryPlayerAdjustSummariesAsync(
        [FromServices] IQueryService<AdjustPlayerSummaryQuery, BrandAdjustSummary> queryService,
        [FromQuery] AdjustRecordQueryViewModel viewModel)
    {
        var query = new AdjustPlayerSummaryQuery(
            (viewModel.From, viewModel.To),
            viewModel.AdjustTypes,
            viewModel.UserId,
            viewModel.Operator);

        return queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Select(summary => new AdjustSummaryListItemViewModel
            {
                BrandId = summary.BrandId,
                Agent = summary.Agent,
                Company = summary.Company,
                System = summary.System,
                Transfer = summary.Transfer,
                Bonus = summary.Bonus
            });
    }

    [HttpGet("summary/agent", Name = "QueryAgentAdjustSummaries")]
    public IAsyncEnumerable<AdjustSummaryListItemViewModel> QueryAgentAdjustSummariesAsync(
        [FromServices] IQueryService<AdjustAgentSummaryQuery, BrandAdjustSummary> queryService,
        [FromQuery] AdjustRecordQueryViewModel viewModel)
    {
        var query = new AdjustAgentSummaryQuery(
            (viewModel.From, viewModel.To),
            viewModel.AdjustTypes,
            string.IsNullOrWhiteSpace(viewModel.UserId)
                ? null
                : Guid.Parse(viewModel.UserId),
            viewModel.Operator);

        return queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Select(summary => new AdjustSummaryListItemViewModel
            {
                BrandId = summary.BrandId,
                Agent = summary.Agent,
                Company = summary.Company,
                System = summary.System,
                Transfer = summary.Transfer,
                Bonus = summary.Bonus,
                AgentOther = summary.AgentOther
            });
    }
}
