using NSubstitute;
using Tgs.CashQueryService;
using Tgs.CashQueryService.ApplicationServices;

namespace CashQuery.Core.UnitTests;

public class AdjustAgentSummaryQueryServiceTests
{
    [Fact]
    public async Task 查詢代理調帳筆數統計資料()
    {
        // Arrange
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustAgentSummaryQueryService(fakeTgsRepository);

        var currentDate = DateTime.Now;

        var query = new AdjustAgentSummaryQuery(
            (From: currentDate.AddDays(-1D), To: currentDate),
            null,
            null,
            null);

        // Act
        _ = await sut.QueryAsync(query).ToArrayAsync();

        // Assert
        _ = fakeTgsRepository.Received(1)
            .QueryAgentAdjustSummariesAsync(
                Arg.Is(new AdjustRecordQueryCondition(
                    query.CreatedOnRange,
                    query.SearchTypes,
                    query.AgentId?.ToString(),
                    query.Operator)));
    }

    [Fact]
    public async Task 計算代理調帳統計筆數()
    {
        // Arrange
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustAgentSummaryQueryService(fakeTgsRepository);

        var currentDate = DateTime.Now;

        var query = new AdjustAgentSummaryQuery(
            (From: currentDate.AddDays(-1D), To: currentDate),
            null,
            null,
            null);

        // Act
        _ = await sut.GetAsync(query);

        // Assert
        _ = fakeTgsRepository.Received(1)
            .CountQueryAgentAdjustSummariesAsync(
                Arg.Is(new AdjustRecordQueryCondition(
                    query.CreatedOnRange,
                    query.SearchTypes,
                    query.AgentId?.ToString(),
                    query.Operator)));
    }
}
