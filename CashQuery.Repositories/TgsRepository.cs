using System.Reactive.Linq;
using Microsoft.EntityFrameworkCore;
using Tgs.CashQuery.Repositories.Entities;
using Tgs.CashQueryService;

namespace Tgs.CashQuery.Repositories;

public class TgsRepository : ITgsRepository
{
    private readonly TgsContext _dbContext;

    public TgsRepository(TgsContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<AdjustRecord> QueryPlayerAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int skip = 0,
        CancellationToken cancellationToken = default)
    {
        var query = BuildPlayerAdjustRecordsQuery(brandId, condition);

        return query
            .OrderByDescending(record => record.CreatedOn)
            .Select(record => new
            {
                record.Id,
                record.BrandId,
                record.PlayerCode,
                record.Type,
                record.AdjustmentType,
                record.Amount,
                record.BucketAmount,
                record.CreatedByUsername,
                record.CreatedOn,
                record.Remark,
            })
            .Skip(skip)
            .Take(1000)
            .ToAsyncEnumerable()
            .Select(row => new AdjustRecord(
                row.Id,
                row.BrandId,
                row.PlayerCode,
                UserType.Player,
                (AdjustType)row.Type,
                (AdjustmentType)row.AdjustmentType,
                row.Amount,
                row.BucketAmount,
                row.CreatedByUsername,
                row.Remark,
                row.CreatedOn));
    }

    public async ValueTask<int> CountQueryPlayerAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default)
        => await BuildPlayerAdjustRecordsQuery(
            brandId,
            condition).CountAsync(cancellationToken).ConfigureAwait(false);

    public IAsyncEnumerable<AdjustRecord> QueryAgentAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int skip = 0,
        CancellationToken ctancellationToken = default)
    {
        var query = BuildAgentAdjustRecordsQuery(brandId, condition);

        return query
            .OrderByDescending(record => record.CreatedOn)
            .Select(record => new
            {
                record.Id,
                record.BrandId,
                record.AgentId,
                record.Type,
                record.AdjustmentType,
                record.Amount,
                record.CreatedByUsername,
                record.CreatedOn,
                record.Remark
            })
            .Skip(skip)
            .Take(1000)
            .ToAsyncEnumerable()
            .Select(row => new AdjustRecord(
                row.Id,
                row.BrandId,
                row.AgentId.ToString(),
                UserType.Agent,
                AdjustType.AgentOther,
                (AdjustmentType)row.AdjustmentType,
                row.Amount,
                0M,
                row.CreatedByUsername,
                row.Remark,
                row.CreatedOn));
    }

    public async ValueTask<int> CountQueryAgentAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default)
        => await BuildAgentAdjustRecordsQuery(
            brandId,
            condition).CountAsync(cancellationToken).ConfigureAwait(false);

    public IAsyncEnumerable<BrandAdjustSummary> QueryPlayerAdjustSummariesAsync(
            AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default)
    {
        var query = BuildPlayerAdjustSummariesQuery(condition);

        return query
            .GroupBy(record => new { record.BrandId, record.Type })
            .OrderBy(g => g.Key.BrandId)
            .Select(g => new
            {
                g.Key.BrandId,
                g.Key.Type,
                Count = g.Count(),
                Amount = g.Sum(record => record.Amount * (record.AdjustmentType == (short)AdjustmentType.Add ? 1 : -1))
            })
            .ToAsyncEnumerable()
            .PivotAsync(
                g => g.BrandId,
                (brandId, records) =>
                {
                    var countMap = new Dictionary<AdjustType, int>(5)
                    {
                        [AdjustType.CompanyAdjust] = 0,
                        [AdjustType.AgentAdjust] = 0,
                        [AdjustType.SystemAdjust] = 0,
                        [AdjustType.TransferAdjust] = 0,
                        [AdjustType.BonusAdjust] = 0
                    };
                    var amountMap = new Dictionary<AdjustType, decimal>(5)
                    {
                        [AdjustType.CompanyAdjust] = 0M,
                        [AdjustType.AgentAdjust] = 0M,
                        [AdjustType.SystemAdjust] = 0M,
                        [AdjustType.TransferAdjust] = 0M,
                        [AdjustType.BonusAdjust] = 0M
                    };

                    foreach (var rec in records)
                    {
                        var adjustType = (AdjustType)rec.Type;

                        countMap[adjustType] = rec.Count;
                        amountMap[adjustType] = rec.Amount;
                    }

                    return new BrandAdjustSummary(
                        brandId,
                        countMap[AdjustType.CompanyAdjust],
                        countMap[AdjustType.AgentAdjust],
                        countMap[AdjustType.SystemAdjust],
                        countMap[AdjustType.TransferAdjust],
                        countMap[AdjustType.BonusAdjust],
                        0,
                        amountMap[AdjustType.CompanyAdjust],
                        amountMap[AdjustType.AgentAdjust],
                        amountMap[AdjustType.SystemAdjust],
                        amountMap[AdjustType.TransferAdjust],
                        amountMap[AdjustType.BonusAdjust],
                        0M);
                });
    }

    public async ValueTask<int> CountQueryPlayerAdjustSummariesAsync(AdjustRecordQueryCondition condition, CancellationToken cancellation = default)
        => await BuildPlayerAdjustSummariesQuery(condition)
            .DistinctBy(record => record.BrandId)
            .CountAsync(cancellation)
            .ConfigureAwait(false);

    public IAsyncEnumerable<BrandAdjustSummary> QueryAgentAdjustSummariesAsync(AdjustRecordQueryCondition condition, CancellationToken cancellationToken = default)
    {
        var query = BuildAgentAdjustSummariesQuery(condition);

        return query
            .GroupBy(record => new { record.BrandId, record.Type })
            .OrderBy(g => g.Key.BrandId)
            .Select(g => new
            {
                g.Key.BrandId,
                Count = g.Count(),
                Amount = g.Sum(record => record.Amount * (record.AdjustmentType == (short)AdjustmentType.Add ? 1 : -1))
            })
            .ToAsyncEnumerable()
            .Select(row => new BrandAdjustSummary(
                row.BrandId,
                0,
                0,
                0,
                0,
                0,
                row.Count,
                0M,
                0M,
                0M,
                0M,
                0M,
                row.Amount));
    }

    public async ValueTask<int> CountQueryAgentAdjustSummariesAsync(AdjustRecordQueryCondition condition, CancellationToken cancellation = default)
        => await BuildAgentAdjustSummariesQuery(condition)
            .DistinctBy(record => record.BrandId)
            .CountAsync(cancellation)
            .ConfigureAwait(false);

    private IQueryable<AdjustAgentRecord> BuildAgentAdjustSummariesQuery(AdjustRecordQueryCondition condition)
    {
        var query = from record in _dbContext.AdjustAgentRecords.AsQueryable()
                    where record.CreatedOn >= condition.CreatedOnRange.From
                        && record.CreatedOn < condition.CreatedOnRange.To
                    select record;

        if (!string.IsNullOrEmpty(condition.UserId))
        {
            var agentId = Guid.Parse(condition.UserId);

            query = query.Where(record => record.AgentId == agentId);
        }

        if (!string.IsNullOrEmpty(condition.Operator))
            query = query.Where(record => record.CreatedByUsername == condition.Operator);

        if (condition.SearchTypes is not null && condition.SearchTypes.Any())
        {
            var searchTypes = condition.SearchTypes.Select(type => (short)type).ToArray();

            query = query.Where(record => searchTypes.Contains(record.Type));
        }

        return query;
    }

    private IQueryable<AdjustPlayerRecord> BuildPlayerAdjustSummariesQuery(AdjustRecordQueryCondition condition)
    {
        var query = from record in _dbContext.AdjustPlayerRecords.AsQueryable()
                    where record.CreatedOn >= condition.CreatedOnRange.From
                        && record.CreatedOn < condition.CreatedOnRange.To
                    select record;

        if (!string.IsNullOrEmpty(condition.UserId))
            query = query.Where(record => record.PlayerCode == condition.UserId);

        if (!string.IsNullOrEmpty(condition.Operator))
            query = query.Where(record => record.CreatedByUsername == condition.Operator);

        if (condition.SearchTypes is not null && condition.SearchTypes.Any())
        {
            var searchTypes = condition.SearchTypes.Select(type => (short)type).ToArray();

            query = query.Where(record => searchTypes.Contains(record.Type));
        }

        return query;
    }

    private IQueryable<AdjustAgentRecord> BuildAgentAdjustRecordsQuery(Guid brandId, AdjustRecordQueryCondition condition)
    {
        var query = from record in _dbContext.AdjustAgentRecords.AsQueryable()
                    where record.BrandId == brandId
                        && record.CreatedOn >= condition.CreatedOnRange.From
                        && record.CreatedOn < condition.CreatedOnRange.To
                    select record;

        if (!string.IsNullOrEmpty(condition.UserId))
        {
            var agentId = Guid.Parse(condition.UserId);

            query = query.Where(record => record.AgentId == agentId);
        }

        if (!string.IsNullOrEmpty(condition.Operator))
            query = query.Where(record => record.CreatedByUsername == condition.Operator);

        if (condition.SearchTypes is not null && condition.SearchTypes.Any())
        {
            var searchTypes = condition.SearchTypes.Select(type => (short)type).ToArray();

            query = query.Where(record => searchTypes.Contains(record.Type));
        }

        return query;
    }

    private IQueryable<AdjustPlayerRecord> BuildPlayerAdjustRecordsQuery(Guid brandId, AdjustRecordQueryCondition condition)
    {
        var query = from record in _dbContext.AdjustPlayerRecords.AsQueryable()
                    where record.BrandId == brandId
                        && record.CreatedOn >= condition.CreatedOnRange.From
                        && record.CreatedOn < condition.CreatedOnRange.To
                    select record;

        if (!string.IsNullOrEmpty(condition.UserId))
            query = query.Where(record => record.PlayerCode == condition.UserId);

        if (!string.IsNullOrEmpty(condition.Operator))
            query = query.Where(record => record.CreatedByUsername == condition.Operator);

        if (condition.SearchTypes is not null && condition.SearchTypes.Any())
        {
            var searchTypes = condition.SearchTypes.Select(type => (short)type).ToArray();

            query = query.Where(record => searchTypes.Contains(record.Type));
        }

        return query;
    }
}

file static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<TRow> PivotAsync<TSource, TKey, TRow>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, IEnumerable<TSource>, TRow> transform)
    {
        var currentKey = default(TKey);
        var collection = new List<TSource>(6);

        await foreach (var item in source)
        {
            var itemKey = keySelector(item);

            if (EqualityComparer<TKey>.Default.Equals(currentKey, default))
                currentKey = itemKey;

            if (!EqualityComparer<TKey>.Default.Equals(currentKey, itemKey))
            {
                yield return transform(currentKey!, collection);

                collection = new List<TSource>(6);
                currentKey = itemKey;
            }

            collection.Add(item);
        }

        if (collection.Count > 0
            && !EqualityComparer<TKey>.Default.Equals(currentKey, default))
            yield return transform(currentKey!, collection);
    }
}
