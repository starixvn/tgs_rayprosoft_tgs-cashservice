using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustPlayerWalletRecordQueryService : IQueryService<AdjustPlayerWalletRecordQuery, AdjustRecord>
{
    private readonly ITgsRepository _tgsRepository;

    public AdjustPlayerWalletRecordQueryService(ITgsRepository tgsRepository)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
    }

    public IAsyncEnumerable<AdjustRecord> QueryAsync(AdjustPlayerWalletRecordQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.QueryPlayerAdjustRecordsAsync(
            new AdjustRecordQueryCondition(
                query.BrandId,
                query.CreatedOnRange,
                query.SearchTypes,
                query.PlayerCode,
                query.Operator),
            query.Take,
            query.Skip,
            cancellationToken);
}
