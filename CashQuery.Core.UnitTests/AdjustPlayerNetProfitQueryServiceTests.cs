using NSubstitute;
using Tgs.CashQueryService;
using Tgs.CashQueryService.ApplicationServices;

namespace CashQuery.Core.UnitTests;

public class AdjustPlayerNetProfitQueryServiceTests
{
    [Fact]
    public async Task 查詢玩家調帳記錄淨營利()
    {
        // Arrange
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustPlayerNetProfitQueryService(fakeTgsRepository);

        var transactionIds = new Guid[]
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        var query = new AdjustPlayerNetProfitQuery(transactionIds);

        // Act
        _ = await sut.QueryAsync(query).ToArrayAsync();

        // Assert
        _ = fakeTgsRepository.Received(1)
            .QueryPlayerAdjustNetProfitAsync(Arg.Is(query.TransactionIds));
    }
}
