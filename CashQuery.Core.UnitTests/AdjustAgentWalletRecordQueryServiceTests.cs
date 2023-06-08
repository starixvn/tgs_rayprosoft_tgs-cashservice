using NSubstitute;
using Tgs.CashQueryService;
using Tgs.CashQueryService.ApplicationServices;

namespace CashQuery.Core.UnitTests;

public class AdjustAgentWalletRecordQueryServiceTests
{
    [Fact]
    public async Task 查詢代理調帳記錄()
    {
        // Arrange
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustAgentWalletRecordQueryService(fakeTgsRepository);

        var query = new AdjustAgentWalletRecordQuery(
            Guid.NewGuid(),
            (From: DateTime.Now, To: DateTime.Now.AddDays(1D)),
            null,
            null,
            null,
            10);

        // Act
        _ = await sut.QueryAsync(query).ToArrayAsync();

        // Assert
        _ = fakeTgsRepository.Received(1)
            .QueryAgentAdjustRecordsAsync(
                Arg.Is(query.BrandId),
                Arg.Is(new AdjustRecordQueryCondition(
                    query.CreatedOnRange,
                    query.SearchTypes,
                    query.AgentId?.ToString(),
                    query.Operator)),
                Arg.Is(query.Skip));
    }

    [Fact]
    public async Task 計算代理調帳記錄筆數()
    {
        // Arrange
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustAgentWalletRecordQueryService(fakeTgsRepository);

        var query = new AdjustAgentWalletRecordQuery(
            Guid.NewGuid(),
            (From: DateTime.Now, To: DateTime.Now.AddDays(1D)),
            null,
            null,
            null,
            10);

        // Act
        _ = await sut.GetAsync(query);

        // Assert
        _ = fakeTgsRepository.Received(1)
            .CountQueryAgentAdjustRecordsAsync(
                Arg.Is(query.BrandId),
                Arg.Is(new AdjustRecordQueryCondition(
                    query.CreatedOnRange,
                    query.SearchTypes,
                    query.AgentId?.ToString(),
                    query.Operator)));
    }
}
