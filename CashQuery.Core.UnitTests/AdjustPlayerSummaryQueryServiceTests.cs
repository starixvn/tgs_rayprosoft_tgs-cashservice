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
            null,
            10,
            0);

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
}
