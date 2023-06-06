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
        AdjustRecordQueryCondition condition,
        int take,
        int skip = 0,
        CancellationToken cancellationToken = default)
    {
        var query = from record in _dbContext.AdjustPlayerRecords.AsQueryable()
                    where record.BrandId == condition.BrandId
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
        AdjustRecordQueryCondition condition,
        int take,
        int skip = 0,
        CancellationToken ctancellationToken = default)
    {
        var query = from record in _dbContext.AdjustAgentRecords.AsQueryable()
                    where record.BrandId == condition.BrandId
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
                UserType.Player,
                (AdjustType)row.Type,
                row.Amount,
                0M,
                row.CreatedByUsername,
                row.Remark,
                row.CreatedOn));
    }
}
