using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tgs.CashQuery.Client;
using Tgs.CashQuery.Repositories.Entities;
using Xunit.Abstractions;

namespace Tgs.CashQueryService.IntegrationTests;

public class AdjustQueryTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public AdjustQueryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
    }

    [Fact]
    public async Task 查詢玩家調帳記錄列表()
    {
        // Arrange
        await using var web = CashQueryTestHost.Factory
            .WithWebHostBuilder(hostBuilder => hostBuilder
                .ConfigureLogging(logging => logging.AddXUnit(_testOutputHelper)));

        var httpClient = web.CreateClient();

        var sut = new CashQueryClient(httpClient);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var brandId = Guid.NewGuid();
        var condition = new CashQuery.Client.AdjustRecordQueryCondition(
            (From: DateTime.Now.AddDays(-1D), To: DateTime.Now.AddHours(1D)),
            null,
            null,
            null);

        var playerRecords = new[]
        {
            new AdjustPlayerRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                Type = (short)AdjustType.CompanyAdjust,
                PlayerCode = "test1",
                Amount = 10M,
                BucketAmount = 10M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.ToUniversalTime(),
                AdjustmentType = 1
            },
            new AdjustPlayerRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                Type = (short)AdjustType.CompanyAdjust,
                PlayerCode = "test2",
                Amount = 20M,
                BucketAmount = 20M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(1D).ToUniversalTime(),
                AdjustmentType = 1
            },
            new AdjustPlayerRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                Type = (short)AdjustType.CompanyAdjust,
                PlayerCode = "test1",
                Amount = 30M,
                BucketAmount = 30M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(2D).ToUniversalTime(),
                AdjustmentType = 1
            }
        };


        await dbContext.AdjustPlayerRecords.AddRangeAsync(playerRecords);

        _ = await dbContext.SaveChangesAsync();

        var expected = new AdjustRecordListItem(
            playerRecords[2].Id,
            playerRecords[2].BrandId,
            playerRecords[2].PlayerCode,
            (AdjustType)playerRecords[2].Type,
            playerRecords[2].Amount,
            playerRecords[2].BucketAmount,
            playerRecords[2].CreatedByUsername,
            playerRecords[2].Remark,
            playerRecords[2].CreatedOn,
            UserType.Player);

        // Act
        var actual = await sut.QueryPlayerAdjustRecordsAsync(brandId, condition, 0, 1).ToArrayAsync();

        // Assert
        _ = Assert.Single(actual);
        Assert.Equal(expected, actual[0]);
    }
}
