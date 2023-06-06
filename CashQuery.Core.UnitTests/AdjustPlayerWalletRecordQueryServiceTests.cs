using NSubstitute;
using Tgs.CashQueryService;
using Tgs.CashQueryService.ApplicationServices;

namespace CashQuery.Core.UnitTests;

public class AdjustPlayerWalletRecordQueryServiceTests
{
    [Fact]
    public async Task 查詢玩家調帳記錄()
    {
        // Arrange
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustPlayerWalletRecordQueryService(fakeTgsRepository);

        var currentDate = DateTime.Now;

        var query = new AdjustPlayerWalletRecordQuery(
            Guid.NewGuid(),
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
            .QueryPlayerAdjustRecordsAsync(
                Arg.Is(new AdjustRecordQueryCondition(
                    query.BrandId,
                    query.CreatedOnRange,
                    query.SearchTypes,
                    query.PlayerCode,
                    query.Operator)),
                Arg.Is(query.Take),
                Arg.Is(query.Skip));
    }
}
