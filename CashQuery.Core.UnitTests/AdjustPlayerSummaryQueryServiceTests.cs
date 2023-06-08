using NSubstitute;
using Tgs.CashQueryService;
using Tgs.CashQueryService.ApplicationServices;

namespace CashQuery.Core.UnitTests;

public class AdjustPlayerSummaryQueryServiceTests
{
    [Fact]
    public async Task 查詢玩家調帳筆數統計資料()
    {
        // Arrange
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustPlayerSummaryQueryService(fakeTgsRepository);

        var currentDate = DateTime.Now;

        var query = new AdjustPlayerSummaryQuery(
            (From: currentDate.AddDays(-1D), To: currentDate),
            null,
            null,
            null);

        // Act
        _ = await sut.QueryAsync(query).ToArrayAsync();

        // Assert
        _ = fakeTgsRepository.Received(1)
            .QueryPlayerAdjustSummariesAsync(
                Arg.Is(new AdjustRecordQueryCondition(
                    query.CreatedOnRange,
                    query.SearchTypes,
                    query.PlayerCode,
                    query.Operator)));
    }

    [Fact]
    public async Task 計算玩家調帳統計筆數()
    {
        // Arrange
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustPlayerSummaryQueryService(fakeTgsRepository);

        var currentDate = DateTime.Now;

        var query = new AdjustPlayerSummaryQuery(
            (From: currentDate.AddDays(-1D), To: currentDate),
            null,
            null,
            null);

        // Act
        _ = await sut.GetAsync(query);

        // Assert
        _ = fakeTgsRepository.Received(1)
            .CountQueryPlayerAdjustSummariesAsync(
                Arg.Is(new AdjustRecordQueryCondition(
                    query.CreatedOnRange,
                    query.SearchTypes,
                    query.PlayerCode,
                    query.Operator)));
    }
}
