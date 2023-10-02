using System.Net;
using Microsoft.AspNetCore.Mvc;
using Tgs.CashQueryService.ApplicationServices;
using Tgs.CashQueryService.ViewModels;
using Tgs.ServiceUtility;
using Tgs.WebApi.Toolkits;

namespace Tgs.CashQueryService.Controllers;

[Route("adjust")]
[ApiController]
public class AdjustQueryController : ControllerBase
{
    [HttpHead("player/{brandId}", Name = "CountPlayerAdjustRecords")]
    [SuccessStatusCode(HttpStatusCode.NoContent)]
    public async Task<NoContentResult> CountPlayerAdjustRecordsAsync(
        [FromServices] IGetService<AdjustPlayerWalletRecordQuery, int> countService,
        Guid brandId,
        [FromQuery] AdjustRecordQueryViewModel viewModel,
        [FromQuery(Name = "skip")] int skip = 0)
    {
        var query = new AdjustPlayerWalletRecordQuery(
            brandId,
            (viewModel.From, viewModel.To),
            viewModel.AdjustTypes,
            viewModel.UserId,
            viewModel.Operator,
            skip);

        var count = await countService.GetAsync(
            query,
            HttpContext.RequestAborted).ConfigureAwait(false);

        HttpContext.Response.Headers.Add("Tgs-Return", count.ToString());

        return NoContent();
    }

    [HttpGet("player/{brandId}", Name = "QueryPlayerAdjustRecords")]
    public IAsyncEnumerable<AdjustRecordListItemViewModel> QueryPlayerAdjustRecordsAsync(
        [FromServices] IQueryService<AdjustPlayerWalletRecordQuery, AdjustRecord> queryService,
        Guid brandId,
        [FromQuery] AdjustRecordQueryViewModel viewModel,
        [FromQuery(Name = "skip")] int skip = 0)
    {
        var query = new AdjustPlayerWalletRecordQuery(
            brandId,
            (viewModel.From, viewModel.To),
            viewModel.AdjustTypes,
            viewModel.UserId,
            viewModel.Operator,
            skip);

        return queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Select(record => new AdjustRecordListItemViewModel
            {
                Id = record.Id,
                AdjustType = record.AdjustType,
                UserId = record.UserId,
                OperationMode = record.OperationMode,
                Amount = record.Amount,
                BrandId = record.BrandId,
                BucketAmount = record.BucketAmount,
                CreatedOn = record.CreatedOn,
                Operator = record.Operator,
                Remark = record.Remark
            });
    }

    [HttpHead("agent/{brandId}", Name = "CountAgentAdjustRecords")]
    [SuccessStatusCode(HttpStatusCode.NoContent)]
    public async Task<NoContentResult> CountAgentAdjustRecordsAsync(
        [FromServices] IGetService<AdjustAgentWalletRecordQuery, int> countService,
        Guid brandId,
        [FromQuery] AdjustRecordQueryViewModel viewModel,
        [FromQuery(Name = "skip")] int skip = 0)
    {
        var query = new AdjustAgentWalletRecordQuery(
            brandId,
            (viewModel.From, viewModel.To),
            viewModel.AdjustTypes,
            string.IsNullOrWhiteSpace(viewModel.UserId)
                ? null
                : Guid.Parse(viewModel.UserId),
            viewModel.Operator,
            skip);

        var count = await countService.GetAsync(
            query,
            HttpContext.RequestAborted).ConfigureAwait(false);

        HttpContext.Response.Headers.Add("Tgs-Return", count.ToString());

        return NoContent();
    }

    [HttpGet("agent/{brandId}", Name = "QueryAgentAdjustRecords")]
    public IAsyncEnumerable<AdjustRecordListItemViewModel> QueryAgentAdjustRecordsAsync(
        [FromServices] IQueryService<AdjustAgentWalletRecordQuery, AdjustRecord> queryService,
        Guid brandId,
        [FromQuery] AdjustRecordQueryViewModel viewModel,
        [FromQuery(Name = "skip")] int skip = 0)
    {
        var query = new AdjustAgentWalletRecordQuery(
            brandId,
            (viewModel.From, viewModel.To),
            viewModel.AdjustTypes,
            string.IsNullOrWhiteSpace(viewModel.UserId)
                ? null
                : Guid.Parse(viewModel.UserId),
            viewModel.Operator,
            skip);

        return queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Select(record => new AdjustRecordListItemViewModel
            {
                Id = record.Id,
                AdjustType = record.AdjustType,
                UserId = record.UserId,
                OperationMode = record.OperationMode,
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
                Bonus = summary.Bonus,
                CompanyAmount = summary.CompanyAmount,
                AgentAmount = summary.AgentAmount,
                SystemAmount = summary.SystemAmount,
                TransferAmount = summary.TransferAmount,
                BonusAmount = summary.BonusAmount
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
                AgentOther = summary.AgentOther,
                AgentOtherAmount = summary.AgentOtherAmount
            });
    }

    [HttpGet("player/netprofit", Name = "QueryPlayerAdjustIncludeInNetProfitRecords")]
    public IAsyncEnumerable<AdjustPlayerRecordViewModel> QueryPlayerAdjustIncludeInNetProfitRecordsAsync(
       [FromServices] IQueryService<AdjustPlayerNetProfitQuery, PlayerAdjustRecord> queryService,
       [FromQuery(Name = "trans")] IEnumerable<Guid> transactionIds)
    {
        var query = new AdjustPlayerNetProfitQuery(transactionIds);

        return queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Select(record => new AdjustPlayerRecordViewModel
            {
                TransactionId = record.TransactionId,
                IncludeInNetProfit = record.IncludeInNetProfit
            });
    }
}
