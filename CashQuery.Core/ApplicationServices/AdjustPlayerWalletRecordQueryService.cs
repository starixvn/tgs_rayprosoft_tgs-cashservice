using Tgs.ServiceUtility;

namespace Tgs.CashQueryService.ApplicationServices;

internal class AdjustPlayerWalletRecordQueryService
    : IQueryService<AdjustPlayerWalletRecordQuery, AdjustRecord>,
    IGetService<AdjustPlayerWalletRecordQuery, int>
{
    private readonly ITgsRepository _tgsRepository;

    public AdjustPlayerWalletRecordQueryService(ITgsRepository tgsRepository)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
    }

    public IAsyncEnumerable<AdjustRecord> QueryAsync(AdjustPlayerWalletRecordQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.QueryPlayerAdjustRecordsAsync(
            query.BrandId,
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.PlayerCode,
                query.Operator),
            query.Skip,
            cancellationToken);

    public ValueTask<int> GetAsync(AdjustPlayerWalletRecordQuery query, CancellationToken cancellationToken = default)
        => _tgsRepository.CountQueryPlayerAdjustRecordsAsync(
            query.BrandId,
            new AdjustRecordQueryCondition(
                query.CreatedOnRange,
                query.SearchTypes,
                query.PlayerCode,
                query.Operator),
            cancellationToken);
}
