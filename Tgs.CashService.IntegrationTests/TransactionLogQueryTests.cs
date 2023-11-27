using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        var playerCode = "test1";
        var from = DateTime.Now.AddDays(-1D);
        var to = DateTime.Now.AddDays(1D);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var transactionLogs = new[] {
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = playerCode,
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

        _ = await dbContext.Wallets.AddAsync(new Wallet
        {
            Id = walletId,
            PlayerCode = playerCode,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            CashType = nameof(CashType.Player)
        });

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
        var playerCode = "test1";
        var from = DateTime.Now.AddDays(-1D);
        var to = DateTime.Now.AddDays(1D);
        var paging = new PagingConditions(2, 1);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var transactionLogs = new[] {
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = playerCode,
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
                PlayerCode = playerCode,
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
                PlayerCode = playerCode,
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

        _ = await dbContext.Wallets.AddAsync(new Wallet
        {
            Id = walletId,
            PlayerCode = playerCode,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            CashType = nameof(CashType.Player)
        });

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
        var playerCode = "test1";
        var from = DateTime.Now.AddDays(-1D);
        var to = DateTime.Now.AddDays(1D);
        var paging = new PagingConditions(1, 1);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var transactionLogs = new[] {
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = playerCode,
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
                PlayerCode = playerCode,
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
                PlayerCode = playerCode,
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

        _ = await dbContext.Wallets.AddAsync(new Wallet
        {
            Id = walletId,
            PlayerCode = playerCode,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            CashType = nameof(CashType.Player)
        });

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
        var playerCode = "test1";
        var from = DateTime.Now.AddDays(-1D);
        var to = DateTime.Now.AddDays(1D);
        var queryTypes = new[] { 2100, 2300 };
        var paging = new PagingConditions(1, 10);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var transactionLogs = new[] {
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = playerCode,
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
                PlayerCode = playerCode,
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
                PlayerCode = playerCode,
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

        _ = await dbContext.Wallets.AddAsync(new Wallet
        {
            Id = walletId,
            PlayerCode = playerCode,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            CashType = nameof(CashType.Player)
        });

        _ = await dbContext.SaveChangesAsync();

        var expected = new[]
        {
            new Client.TransactionLog(
                transactionLogs[2].Id,
                transactionLogs[2].RecordedOn,
                transactionLogs[2].Type,
                transactionLogs[2].Amount,
                transactionLogs[2].Balance,
                transactionLogs[2].LockAmount,
                transactionLogs[2].LockedBalance,
                transactionLogs[2].Description,
                transactionLogs[2].ExternalTransactionId),
            new Client.TransactionLog(
                transactionLogs[0].Id,
                transactionLogs[0].RecordedOn,
                transactionLogs[0].Type,
                transactionLogs[0].Amount,
                transactionLogs[0].Balance,
                transactionLogs[0].LockAmount,
                transactionLogs[0].LockedBalance,
                transactionLogs[0].Description,
                transactionLogs[0].ExternalTransactionId)
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

    [TestMethod]
    public async Task 查詢玩家遊戲帳變紀錄()
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
        var playerCode = "test1";
        var from = DateTime.Now.AddDays(-1D);
        var to = DateTime.Now.AddDays(1D);

        var dbContext = web.Services.GetRequiredService<TgsContext>();
        var gameActionLogs = new[]
        {
            new GameAction
            {
                Id= Guid.NewGuid(),
                Amount= 100,
                Balance= 900,
                Type = 0,
                RecordedOn = DateTime.UtcNow,
                ExternalBetId = "aaa",
                ExternalTransactionId = "aaa123",
                PlayerCode = playerCode,
                PlayerIp = "127.0.0.1",
                ExternalGameActionGroupId = "aaa"
            }
        };

        await dbContext.GameActions.AddRangeAsync(gameActionLogs);

        _ = await dbContext.Wallets.AddAsync(new Wallet
        {
            Id = walletId,
            PlayerCode = playerCode,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            CashType = nameof(CashType.Player)
        });

        _ = await dbContext.SaveChangesAsync();

        var expected = gameActionLogs
            .Select(trx => new Client.TransactionLog(
                trx.Id,
                trx.RecordedOn,
                (int)TransactionType.GameBet,
                trx.Amount,
                trx.Balance,
                0,
                0,
                null,
                null))
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
    public async Task 查詢玩家帳變紀錄總金額()
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
        var playerCode = "test1";
        var from = DateTime.Now.AddDays(-1D);
        var to = DateTime.Now.AddDays(1D);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var transactionLogs = new[] {
            new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerCode = playerCode,
                RecordedOn = DateTime.UtcNow,
                Amount = 100M,
                Balance = 900M,
                BrandId= Guid.NewGuid(),
                WalletId = walletId,
                LockAmount = 0M,
                LockedBalance = 0M,
                Type = 1001
            }
        };

        await dbContext.Transactions.AddRangeAsync(transactionLogs);

        _ = await dbContext.Wallets.AddAsync(new Wallet
        {
            Id = walletId,
            PlayerCode = playerCode,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            CashType = nameof(CashType.Player)
        });

        _ = await dbContext.SaveChangesAsync();

        var actual = await sut.QueryTransactionTotalAmountAsync(
            walletId,
            from,
            to,
            Array.Empty<int>());

        Assert.AreEqual(100M, actual);
    }

    [TestMethod]
    public async Task 查詢紀錄如果沒有符合的資料應該回傳空陣列並且TotalCount為0()
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
        var playerCode = "test1";
        var from = DateTime.Now.AddDays(-1D);
        var to = DateTime.Now.AddDays(1D);

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        _ = await dbContext.Wallets.AddAsync(new Wallet
        {
            Id = walletId,
            PlayerCode = playerCode,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            CashType = nameof(CashType.Player)
        });

        _ = await dbContext.SaveChangesAsync();

        var actual = await sut.QueryTransactionLogsAsync(
            walletId,
            from,
            to,
            Array.Empty<int>(),
            new PagingConditions(1, 10));

        Assert.AreEqual(0, actual.TotalCount);
        Assert.AreEqual(1, actual.Page);
        Assert.AreEqual(0, actual.Items.Count());
    }

    [TestMethod]
    public async Task 如果查詢不到錢包資料則回傳空陣列()
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

        _ = await dbContext.SaveChangesAsync();

        var actual = await sut.QueryTransactionLogsAsync(
            walletId,
            from,
            to,
            Array.Empty<int>(),
            new PagingConditions(1, 10));

        Assert.AreEqual(0, actual.TotalCount);
        Assert.AreEqual(1, actual.Page);
        Assert.AreEqual(0, actual.Items.Count());
    }
}
