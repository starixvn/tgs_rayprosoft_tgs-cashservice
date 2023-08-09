using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tgs.CashQuery.Repositories.Entities;
using Tgs.CashQueryService.Client;
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
        var condition = new Tgs.CashQueryService.Client.AdjustRecordQueryCondition(
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

        var expected = new[]{
            new AdjustRecordListItem(
                playerRecords[2].Id,
                playerRecords[2].BrandId,
                playerRecords[2].PlayerCode,
                (AdjustType)playerRecords[2].Type,
                (OperationMode)playerRecords[2].AdjustmentType,
                playerRecords[2].Amount,
                playerRecords[2].BucketAmount,
                playerRecords[2].CreatedByUsername,
                playerRecords[2].Remark,
                playerRecords[2].CreatedOn,
                UserType.Player),
            new AdjustRecordListItem(
                playerRecords[1].Id,
                playerRecords[1].BrandId,
                playerRecords[1].PlayerCode,
                (AdjustType)playerRecords[1].Type,
                (OperationMode)playerRecords[1].AdjustmentType,
                playerRecords[1].Amount,
                playerRecords[1].BucketAmount,
                playerRecords[1].CreatedByUsername,
                playerRecords[1].Remark,
                playerRecords[1].CreatedOn,
                UserType.Player),
            new AdjustRecordListItem(
                playerRecords[0].Id,
                playerRecords[0].BrandId,
                playerRecords[0].PlayerCode,
                (AdjustType)playerRecords[0].Type,
                (OperationMode)playerRecords[0].AdjustmentType,
                playerRecords[0].Amount,
                playerRecords[0].BucketAmount,
                playerRecords[0].CreatedByUsername,
                playerRecords[0].Remark,
                playerRecords[0].CreatedOn,
                UserType.Player)
        };

        // Act
        var actual = await sut.QueryPlayerAdjustRecordsAsync(brandId, condition, 3).ToArrayAsync();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task 取得玩家調帳記錄筆數()
    {
        // Arrange
        await using var web = CashQueryTestHost.Factory
            .WithWebHostBuilder(hostBuilder => hostBuilder
                .ConfigureLogging(logging => logging.AddXUnit(_testOutputHelper)));

        var httpClient = web.CreateClient();

        var sut = new CashQueryClient(httpClient);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var brandId = Guid.NewGuid();
        var condition = new Tgs.CashQueryService.Client.AdjustRecordQueryCondition(
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

        // Act
        var actual = await sut.CountPlayerAdjustRecordsAsync(brandId, condition);

        // Assert
        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task 查詢代理調帳記錄列表()
    {
        // Arrange
        await using var web = CashQueryTestHost.Factory
            .WithWebHostBuilder(hostBuilder => hostBuilder
                .ConfigureLogging(logging => logging.AddXUnit(_testOutputHelper)));

        var httpClient = web.CreateClient();

        var sut = new CashQueryClient(httpClient);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var brandId = Guid.NewGuid();
        var condition = new Tgs.CashQueryService.Client.AdjustRecordQueryCondition(
            (From: DateTime.Now.AddDays(-1D), To: DateTime.Now.AddHours(1D)),
            null,
            null,
            null);

        var agentRecords = new[]
        {
            new AdjustAgentRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                AgentId = Guid.NewGuid(),
                Amount = 10M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.ToUniversalTime(),
                AdjustmentType = 1
            },
            new AdjustAgentRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                AgentId = Guid.NewGuid(),
                Amount = 20M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(1D).ToUniversalTime(),
                AdjustmentType = 1
            },
            new AdjustAgentRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                AgentId = Guid.NewGuid(),
                Amount = 30M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(2D).ToUniversalTime(),
                AdjustmentType = 1
            }
        };

        await dbContext.AdjustAgentRecords.AddRangeAsync(agentRecords);

        _ = await dbContext.SaveChangesAsync();

        var expected = new[] {
            new AdjustRecordListItem(
                agentRecords[2].Id,
                agentRecords[2].BrandId,
                agentRecords[2].AgentId.ToString(),
                AdjustType.AgentOther,
                (OperationMode)agentRecords[2].AdjustmentType,
                agentRecords[2].Amount,
                0M,
                agentRecords[2].CreatedByUsername,
                agentRecords[2].Remark,
                agentRecords[2].CreatedOn,
                UserType.Agent),
            new AdjustRecordListItem(
                agentRecords[1].Id,
                agentRecords[1].BrandId,
                agentRecords[1].AgentId.ToString(),
                AdjustType.AgentOther,
                (OperationMode)agentRecords[1].AdjustmentType,
                agentRecords[1].Amount,
                0M,
                agentRecords[1].CreatedByUsername,
                agentRecords[1].Remark,
                agentRecords[1].CreatedOn,
                UserType.Agent),
            new AdjustRecordListItem(
                agentRecords[0].Id,
                agentRecords[0].BrandId,
                agentRecords[0].AgentId.ToString(),
                AdjustType.AgentOther,
                (OperationMode)agentRecords[0].AdjustmentType,
                agentRecords[0].Amount,
                0M,
                agentRecords[0].CreatedByUsername,
                agentRecords[0].Remark,
                agentRecords[0].CreatedOn,
                UserType.Agent)
        };

        // Act
        var actual = await sut.QueryAgentAdjustRecordsAsync(brandId, condition, 3).ToArrayAsync();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task 取得代理調帳記錄筆數()
    {
        // Arrange
        await using var web = CashQueryTestHost.Factory
            .WithWebHostBuilder(hostBuilder => hostBuilder
                .ConfigureLogging(logging => logging.AddXUnit(_testOutputHelper)));

        var httpClient = web.CreateClient();

        var sut = new CashQueryClient(httpClient);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var brandId = Guid.NewGuid();
        var condition = new Client.AdjustRecordQueryCondition(
            (From: DateTime.Now.AddDays(-1D), To: DateTime.Now.AddHours(1D)),
            null,
            null,
            null);

        var agentRecords = new[]
        {
            new AdjustAgentRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                AgentId = Guid.NewGuid(),
                Amount = 10M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.ToUniversalTime(),
                AdjustmentType = 1
            },
            new AdjustAgentRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                AgentId = Guid.NewGuid(),
                Amount = 20M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(1D).ToUniversalTime(),
                AdjustmentType = 1
            },
            new AdjustAgentRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                AgentId = Guid.NewGuid(),
                Amount = 30M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(2D).ToUniversalTime(),
                AdjustmentType = 1
            }
        };

        await dbContext.AdjustAgentRecords.AddRangeAsync(agentRecords);

        _ = await dbContext.SaveChangesAsync();

        // Act
        var actual = await sut.CountAgentAdjustRecordsAsync(brandId, condition);

        // Assert
        Assert.Equal(3, actual);
    }

    [Fact]
    public async Task 查詢玩家調帳總覽()
    {
        // Arrange
        await using var web = CashQueryTestHost.Factory
            .WithWebHostBuilder(hostBuilder => hostBuilder
                .ConfigureLogging(logging => logging.AddXUnit(_testOutputHelper)));

        var httpClient = web.CreateClient();

        var sut = new CashQueryClient(httpClient);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var brandId = Guid.NewGuid();
        var brandId2 = Guid.NewGuid();
        var condition = new Tgs.CashQueryService.Client.AdjustRecordQueryCondition(
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
                Amount = 20M,
                BucketAmount = 20M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.ToUniversalTime(),
                AdjustmentType = (short)OperationMode.Add,
            },
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
                AdjustmentType = (short)OperationMode.Subtract,
            },
            new AdjustPlayerRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                Type = (short)AdjustType.AgentAdjust,
                PlayerCode = "test2",
                Amount = 30M,
                BucketAmount = 30M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(1D).ToUniversalTime(),
                AdjustmentType = (short)OperationMode.Add,
            },
            new AdjustPlayerRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                Type = (short)AdjustType.AgentAdjust,
                PlayerCode = "test2",
                Amount = 20M,
                BucketAmount = 20M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(1D).ToUniversalTime(),
                AdjustmentType = (short)OperationMode.Add,
            },
            new AdjustPlayerRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                Type = (short)AdjustType.AgentAdjust,
                PlayerCode = "test2",
                Amount = 10M,
                BucketAmount = 10M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(1D).ToUniversalTime(),
                AdjustmentType = (short)OperationMode.Subtract,
            },
            new AdjustPlayerRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId2,
                Type = (short)AdjustType.CompanyAdjust,
                PlayerCode = "test1",
                Amount = 30M,
                BucketAmount = 30M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(2D).ToUniversalTime(),
                AdjustmentType = (short)OperationMode.Add,
            }
        };

        await dbContext.AdjustPlayerRecords.AddRangeAsync(playerRecords);

        _ = await dbContext.SaveChangesAsync();

        var expected = new[]{
            new AdjustSummaryListItem(
                brandId,
                2,
                3,
                0,
                0,
                0,
                0,
                10M,
                40M,
                0M,
                0M,
                0M,
                0M),
            new AdjustSummaryListItem(
                brandId2,
                1,
                0,
                0,
                0,
                0,
                0,
                30M,
                0M,
                0M,
                0M,
                0M,
                0M)
        };

        // Act
        var actual = await sut.QueryPlayerAdjustSummariesAsync(condition).ToArrayAsync();

        // Assert
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async Task 查詢代理調帳總覽()
    {
        // Arrange
        await using var web = CashQueryTestHost.Factory
            .WithWebHostBuilder(hostBuilder => hostBuilder
                .ConfigureLogging(logging => logging.AddXUnit(_testOutputHelper)));

        var httpClient = web.CreateClient();

        var sut = new CashQueryClient(httpClient);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var brandId = Guid.NewGuid();
        var brandId2 = Guid.NewGuid();
        var condition = new Tgs.CashQueryService.Client.AdjustRecordQueryCondition(
            (From: DateTime.Now.AddDays(-1D), To: DateTime.Now.AddHours(1D)),
            null,
            null,
            null);

        var agentRecords = new[]
        {
            new AdjustAgentRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                AgentId = Guid.NewGuid(),
                Amount = 30M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.ToUniversalTime(),
                AdjustmentType = (short)OperationMode.Add
            },
            new AdjustAgentRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                AgentId = Guid.NewGuid(),
                Amount = 20M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(1D).ToUniversalTime(),
                AdjustmentType = (short)OperationMode.Add
            },
            new AdjustAgentRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                AgentId = Guid.NewGuid(),
                Amount = 10M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(1D).ToUniversalTime(),
                AdjustmentType = (short) OperationMode.Subtract
            },
            new AdjustAgentRecord
            {
                Id = Guid.NewGuid(),
                BrandId = brandId2,
                AgentId = Guid.NewGuid(),
                Amount = 30M,
                CreatedByUsername = "system",
                CreatedOn = DateTime.Now.AddSeconds(2D).ToUniversalTime(),
                AdjustmentType = (short)OperationMode.Add
            }
        };

        await dbContext.AdjustAgentRecords.AddRangeAsync(agentRecords);

        _ = await dbContext.SaveChangesAsync();

        var expected = new[]{
            new AdjustSummaryListItem(
                brandId,
                0,
                0,
                0,
                0,
                0,
                3,
                0M,
                0M,
                0M,
                0M,
                0M,
                40M),
            new AdjustSummaryListItem(
                brandId2,
                0,
                0,
                0,
                0,
                0,
                1,
                0M,
                0M,
                0M,
                0M,
                0M,
                30M)
        };

        // Act
        var actual = await sut.QueryAgentAdjustSummariesAsync(condition).ToArrayAsync();

        // Assert
        Assert.Equivalent(expected, actual);
    }
}
