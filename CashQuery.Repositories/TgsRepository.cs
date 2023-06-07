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
        int take,
        int skip = 0,
        CancellationToken cancellationToken = default)
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

        return query
            .OrderByDescending(record => record.CreatedOn)
            .Select(record => new
            {
                record.Id,
                record.BrandId,
                record.PlayerCode,
                record.Type,
                record.Amount,
                record.BucketAmount,
                record.CreatedByUsername,
                record.CreatedOn,
                record.Remark
            })
            .Skip(skip)
            .Take(take)
            .ToAsyncEnumerable()
            .Select(row => new AdjustRecord(
                row.Id,
                row.BrandId,
                row.PlayerCode,
                UserType.Player,
                (AdjustType)row.Type,
                row.Amount,
                row.BucketAmount,
                row.CreatedByUsername,
                row.Remark,
                row.CreatedOn));
    }

    public IAsyncEnumerable<AdjustRecord> QueryAgentAdjustRecordsAsync(
        Guid brandId,
        AdjustRecordQueryCondition condition,
        int take,
        int skip = 0,
        CancellationToken ctancellationToken = default)
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

        return query
            .OrderByDescending(record => record.CreatedOn)
            .Select(record => new
            {
                record.Id,
                record.BrandId,
                record.AgentId,
                record.Type,
                record.Amount,
                record.CreatedByUsername,
                record.CreatedOn,
                record.Remark
            })
            .Skip(skip)
            .Take(take)
            .ToAsyncEnumerable()
            .Select(row => new AdjustRecord(
                row.Id,
                row.BrandId,
                row.AgentId.ToString(),
                UserType.Agent,
                AdjustType.AgentOther,
                row.Amount,
                0M,
                row.CreatedByUsername,
                row.Remark,
                row.CreatedOn));
    }

    public IAsyncEnumerable<BrandAdjustSummary> QueryPlayerAdjustSummariesAsync(
        AdjustRecordQueryCondition condition,
        CancellationToken cancellationToken = default)
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

        return query
            .GroupBy(record => new { record.BrandId, record.Type })
            .OrderBy(g => g.Key.BrandId)
            .Select(g => new
            {
                g.Key.BrandId,
                g.Key.Type,
                Count = g.Count()
            })
            .ToAsyncEnumerable()
            .PivotAsync(
                g => g.BrandId,
                (brandId, records) =>
                {
                    var dict = new Dictionary<AdjustType, int>(5)
                    {
                        [AdjustType.CompanyAdjust] = 0,
                        [AdjustType.AgentAdjust] = 0,
                        [AdjustType.SystemAdjust] = 0,
                        [AdjustType.TransferAdjust] = 0,
                        [AdjustType.BonusAdjust] = 0
                    };

                    foreach (var rec in records)
                    {
                        var adjustType = (AdjustType)rec.Type;

                        dict[adjustType] = rec.Count;
                    }

                    return new BrandAdjustSummary(
                        brandId,
                        dict[AdjustType.CompanyAdjust],
                        dict[AdjustType.AgentAdjust],
                        dict[AdjustType.SystemAdjust],
                        dict[AdjustType.TransferAdjust],
                        dict[AdjustType.BonusAdjust],
                        0);
                });
    }

    public IAsyncEnumerable<BrandAdjustSummary> QueryAgentAdjustSummariesAsync(AdjustRecordQueryCondition condition, CancellationToken cancellationToken = default)
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

        return query
            .GroupBy(record => new { record.BrandId, record.Type })
            .OrderBy(g => g.Key.BrandId)
            .Select(g => new
            {
                g.Key.BrandId,
                Count = g.Count()
            })
            .ToAsyncEnumerable()
            .Select(row => new BrandAdjustSummary(
                row.BrandId,
                0,
                0,
                0,
                0,
                0,
                row.Count));
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
