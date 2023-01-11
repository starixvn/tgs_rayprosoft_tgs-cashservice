using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tgs.CashService.Client;
using Tgs.CashService.Repositories.Entities;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class TransactionLogQueryTests
{
    [TestMethod]
    public async Task 查詢玩家帳變紀錄()
    {
        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services => services
                .Replace(ServiceDescriptor.Singleton(sp =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
                    _ = dbContextOptionsBuilder
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .ConfigureWarnings(x => x.Ignore(
                            InMemoryEventId.TransactionIgnoredWarning));

                    return new TgsContext(dbContextOptionsBuilder.Options);
                }))));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var walletId = Guid.NewGuid();
        var from = DateTime.Now.AddDays(-1D);
        var to = DateTime.Now.AddDays(1D);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var transactionLogs = new[] {
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow,
                Amount = 100,
                Balance = 900,
                BrandId= Guid.NewGuid(),
                WalletId = walletId,
                LockAmount = 0M,
                LockedBalance = 0M,
                Type = 1001
            }
        };

        await dbContext.Transactions.AddRangeAsync(transactionLogs);

        _ = await dbContext.SaveChangesAsync();

        var expected = transactionLogs
            .Select(trx => new Client.TransactionLog(
                trx.Id,
                trx.RecordedOn,
                trx.Type,
                trx.Amount,
                trx.Balance,
                trx.LockAmount,
                trx.LockedBalance,
                trx.Description,
                trx.ExternalTransactionId))
            .ToArray();

        var actual = await sut.QueryTransactionLogsAsync(
            walletId,
            from,
            to,
            Array.Empty<int>(),
            new PagingConditions(1, 10));

        Assert.AreEqual(1, actual.TotalCount);
        Assert.AreEqual(1, actual.Page);
        CollectionAssert.AreEqual(expected, actual.Items.ToArray());
    }

    [TestMethod]
    public async Task 查詢玩家帳變紀錄分頁功能()
    {
        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services => services
                .Replace(ServiceDescriptor.Singleton(sp =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
                    _ = dbContextOptionsBuilder
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .ConfigureWarnings(x => x.Ignore(
                            InMemoryEventId.TransactionIgnoredWarning));

                    return new TgsContext(dbContextOptionsBuilder.Options);
                }))));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var walletId = Guid.NewGuid();
        var from = DateTime.Now.AddDays(-1D);
        var to = DateTime.Now.AddDays(1D);
        var paging = new PagingConditions(2, 1);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var transactionLogs = new[] {
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow,
                Amount = 100,
                Balance = 900,
                BrandId= Guid.NewGuid(),
                WalletId = walletId,
                LockAmount = 0M,
                LockedBalance = 0M,
                Type = 1001
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow.AddSeconds(1),
                Amount = 200,
                Balance = 900,
                BrandId= Guid.NewGuid(),
                WalletId = walletId,
                LockAmount = 0M,
                LockedBalance = 0M,
                Type = 1001
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow.AddSeconds(2),
                Amount = 300,
                Balance = 900,
                BrandId= Guid.NewGuid(),
                WalletId = walletId,
                LockAmount = 0M,
                LockedBalance = 0M,
                Type = 1001
            }
        };

        await dbContext.Transactions.AddRangeAsync(transactionLogs);

        _ = await dbContext.SaveChangesAsync();

        var expected = new[]
        {
            new Client.TransactionLog(
                transactionLogs[1].Id,
                transactionLogs[1].RecordedOn,
                transactionLogs[1].Type,
                transactionLogs[1].Amount,
                transactionLogs[1].Balance,
                transactionLogs[1].LockAmount,
                transactionLogs[1].LockedBalance,
                transactionLogs[1].Description,
                transactionLogs[1].ExternalTransactionId)
        };

        var actual = await sut.QueryTransactionLogsAsync(
            walletId,
            from,
            to,
            Array.Empty<int>(),
            paging);

        Assert.AreEqual(3, actual.TotalCount);
        Assert.AreEqual(2, actual.Page);
        CollectionAssert.AreEqual(expected, actual.Items.ToArray());
    }

    [TestMethod]
    public async Task 查詢玩家帳變紀錄_查詢日期區間()
    {
        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services => services
                .Replace(ServiceDescriptor.Singleton(sp =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
                    _ = dbContextOptionsBuilder
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .ConfigureWarnings(x => x.Ignore(
                            InMemoryEventId.TransactionIgnoredWarning));

                    return new TgsContext(dbContextOptionsBuilder.Options);
                }))));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var walletId = Guid.NewGuid();
        var from = DateTime.Now.AddDays(-1D);
        var to = DateTime.Now.AddDays(1D);
        var paging = new PagingConditions(1, 1);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var transactionLogs = new[] {
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow.AddDays(-2D),
                Amount = 100,
                Balance = 900,
                BrandId= Guid.NewGuid(),
                WalletId = walletId,
                LockAmount = 0M,
                LockedBalance = 0M,
                Type = 1001
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow.AddSeconds(1),
                Amount = 200,
                Balance = 900,
                BrandId= Guid.NewGuid(),
                WalletId = walletId,
                LockAmount = 0M,
                LockedBalance = 0M,
                Type = 1001
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow.AddDays(2D),
                Amount = 300,
                Balance = 900,
                BrandId= Guid.NewGuid(),
                WalletId = walletId,
                LockAmount = 0M,
                LockedBalance = 0M,
                Type = 1001
            }
        };

        await dbContext.Transactions.AddRangeAsync(transactionLogs);

        _ = await dbContext.SaveChangesAsync();

        var expected = new[]
        {
            new Client.TransactionLog(
                transactionLogs[1].Id,
                transactionLogs[1].RecordedOn,
                transactionLogs[1].Type,
                transactionLogs[1].Amount,
                transactionLogs[1].Balance,
                transactionLogs[1].LockAmount,
                transactionLogs[1].LockedBalance,
                transactionLogs[1].Description,
                transactionLogs[1].ExternalTransactionId)
        };

        var actual = await sut.QueryTransactionLogsAsync(
            walletId,
            from,
            to,
            Array.Empty<int>(),
            paging);

        Assert.AreEqual(1, actual.TotalCount);
        Assert.AreEqual(1, actual.Page);
        CollectionAssert.AreEqual(expected, actual.Items.ToArray());
    }

    [TestMethod]
    public async Task 查詢玩家帳變紀錄_給予Type查詢應該回傳符合的資料()
    {
        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services => services
                .Replace(ServiceDescriptor.Singleton(sp =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
                    _ = dbContextOptionsBuilder
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .ConfigureWarnings(x => x.Ignore(
                            InMemoryEventId.TransactionIgnoredWarning));

                    return new TgsContext(dbContextOptionsBuilder.Options);
                }))));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var walletId = Guid.NewGuid();
        var from = DateTime.Now.AddDays(-1D);
        var to = DateTime.Now.AddDays(1D);
        var queryTypes = new[] { 2100, 2300 };
        var paging = new PagingConditions(1, 10);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var transactionLogs = new[] {
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow,
                Amount = 100,
                Balance = 900,
                BrandId= Guid.NewGuid(),
                WalletId = walletId,
                LockAmount = 0M,
                LockedBalance = 0M,
                Type = 2100
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow.AddSeconds(1),
                Amount = 200,
                Balance = 900,
                BrandId= Guid.NewGuid(),
                WalletId = walletId,
                LockAmount = 0M,
                LockedBalance = 0M,
                Type = 2200
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow.AddSeconds(2),
                Amount = 300,
                Balance = 900,
                BrandId= Guid.NewGuid(),
                WalletId = walletId,
                LockAmount = 0M,
                LockedBalance = 0M,
                Type = 2300
            }
        };

        await dbContext.Transactions.AddRangeAsync(transactionLogs);

        _ = await dbContext.SaveChangesAsync();

        var expected = new[]
        {
            new Client.TransactionLog(
                transactionLogs[0].Id,
                transactionLogs[0].RecordedOn,
                transactionLogs[0].Type,
                transactionLogs[0].Amount,
                transactionLogs[0].Balance,
                transactionLogs[0].LockAmount,
                transactionLogs[0].LockedBalance,
                transactionLogs[0].Description,
                transactionLogs[0].ExternalTransactionId),
            new Client.TransactionLog(
                transactionLogs[2].Id,
                transactionLogs[2].RecordedOn,
                transactionLogs[2].Type,
                transactionLogs[2].Amount,
                transactionLogs[2].Balance,
                transactionLogs[2].LockAmount,
                transactionLogs[2].LockedBalance,
                transactionLogs[2].Description,
                transactionLogs[2].ExternalTransactionId)
        };

        var actual = await sut.QueryTransactionLogsAsync(
            walletId,
            from,
            to,
            queryTypes,
            paging);

        Assert.AreEqual(2, actual.TotalCount);
        Assert.AreEqual(1, actual.Page);
        CollectionAssert.AreEqual(expected, actual.Items.ToArray());
    }
}
