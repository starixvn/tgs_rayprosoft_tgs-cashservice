using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using StackExchange.Redis;
using Tgs.CashService;
using Tgs.CashService.Repositories;
using TGS.GameProviders.Repositories.Entities;

namespace Tgs.CashServiceWorker.Repositories.UnitTests;

public class WalletRepositoryTests
{
    [Fact]
    public async Task 取得錢包餘額()
    {
        var walletId = Guid.NewGuid();
        var balance = 0M;

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        _ = dbContext.WalletItems.Add(new WalletItem
        {
            Id = walletId,
            Balance = balance,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            PlayerCode = "tester",
            CashType = nameof(CashType.Player)
        });

        _ = await dbContext.SaveChangesAsync();

        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new WalletRepository(
            dbContext,
            fakeRedisDatabase);

        var actual = await sut.GetBalanceAsync(walletId);

        Assert.Equal(balance, actual);
    }

    [Fact]
    public async Task 取得錢包資料_如果BrandId欄位是Null_會從玩家資料表查詢_並且更新BrandId欄位()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new WalletRepository(
            dbContext,
            fakeRedisDatabase);

        var walletId = Guid.NewGuid();
        var playerCode = "tester";
        var brandId = Guid.NewGuid();

        _ = await dbContext.WalletItems.AddAsync(new WalletItem
        {
            Id = walletId,
            Balance = 0M,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            PlayerCode = playerCode,
            CashType = nameof(CashType.Player)
        });

        _ = await dbContext.Players.AddAsync(new Player
        {
            PlayerCode = playerCode,
            BrandId = brandId,
            DefaultWalletCurrencyCode = "CNY",
            Password = [],
            Salt = [],
            Username = "abcd",
            WithdrawPassword = [],
            WithdrawSalt = []
        });

        _ = await dbContext.SaveChangesAsync();

        var actual = await sut.GetWalletAsync(walletId);
        var row = await dbContext.WalletItems.FindAsync(walletId);

        Assert.NotNull(actual);
        Assert.Equal(brandId, actual.BrandId);
        Assert.Equal(brandId, row?.BrandId);
    }

    [Fact]
    public async Task 取得錢包資料_如果BrandId有值_會直接使用其值不會再從玩家資料表查詢()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new WalletRepository(
            dbContext,
            fakeRedisDatabase);

        var walletId = Guid.NewGuid();
        var playerCode = "tester";
        var brandId = Guid.NewGuid();

        _ = await dbContext.WalletItems.AddAsync(new WalletItem
        {
            Id = walletId,
            Balance = 0M,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            PlayerCode = playerCode,
            BrandId = brandId,
            CashType = nameof(CashType.Agent)
        });

        _ = await dbContext.SaveChangesAsync();

        var actual = await sut.GetWalletAsync(walletId);

        Assert.NotNull(actual);
        Assert.Equal(brandId, actual.BrandId);
    }

    [Fact]
    public async Task 取得最新交易記錄_如果SanpshotId是空的但是有快照記錄_會取得最新的快照記錄()
    {
        var walletId = Guid.NewGuid();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        _ = await dbContext.Transactions.AddAsync(
            new Transaction
            {
                Id = Guid.NewGuid(),
                Type = (short)TransactionType.Snapshot,
                Balance = 11111M,
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow,
                WalletId = walletId
            });

        _ = await dbContext.SaveChangesAsync();

        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new WalletRepository(
            dbContext,
            fakeRedisDatabase);

        var actual = await sut.GetNewestRecordAsync(walletId, null);

        Assert.NotNull(actual);
        Assert.Equal(11111M, actual.Balance);
    }

    [Fact]
    public async Task 取得最新交易記錄_會取得MigrateDate之後的記錄()
    {
        var walletId = Guid.NewGuid();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        await dbContext.Transactions.AddRangeAsync(
            new Transaction
            {
                Id = Guid.NewGuid(),
                Type = (short)TransactionType.Snapshot,
                Balance = 11111M,
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow,
                WalletId = walletId
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Type = (short)TransactionType.Snapshot,
                Balance = 333M,
                PlayerCode = "test1",
                RecordedOn = DateTime.Parse("2023-11-19").ToUniversalTime(),
                WalletId = walletId
            });

        _ = await dbContext.SaveChangesAsync();

        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new WalletRepository(
            dbContext,
            fakeRedisDatabase);

        var actual = await sut.GetNewestRecordAsync(walletId, null);

        Assert.NotNull(actual);
        Assert.Equal(11111M, actual.Balance);
    }

    [Fact]
    public async Task 取得最新交易記錄_會取得指定SnapshotId之後的最新記錄()
    {
        var walletId = Guid.NewGuid();
        var snapshotId = Guid.Parse("018c3e61-4bfa-0000-0001-07e731f8bf36");

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        await dbContext.Transactions.AddRangeAsync(
            new Transaction
            {
                Id = Guid.Parse("018c3e6e-28c0-0000-0001-07e731f8bf36"),
                Type = (short)TransactionType.Snapshot,
                Balance = 11111M,
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow,
                WalletId = walletId
            },
            new Transaction
            {
                Id = Guid.Parse("018c3e57-124e-0000-0001-07e731f8bf36"),
                Type = (short)TransactionType.Snapshot,
                Balance = 333M,
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow,
                WalletId = walletId
            });

        _ = await dbContext.SaveChangesAsync();

        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new WalletRepository(
            dbContext,
            fakeRedisDatabase);

        var actual = await sut.GetNewestRecordAsync(walletId, snapshotId);

        Assert.NotNull(actual);
        Assert.Equal(11111M, actual.Balance);
    }

    [Fact]
    public async Task 取得最新交易記錄_如果沒有新記錄會回傳Null()
    {
        var walletId = Guid.NewGuid();
        var snapshotId = Guid.Parse("018c3e61-4bfa-0000-0001-07e731f8bf36");

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        await dbContext.Transactions.AddRangeAsync(
            new Transaction
            {
                Id = Guid.Parse("018c3e57-124e-0000-0001-07e731f8bf36"),
                Type = (short)TransactionType.Snapshot,
                Balance = 333M,
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow,
                WalletId = walletId
            });

        _ = await dbContext.SaveChangesAsync();

        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new WalletRepository(
            dbContext,
            fakeRedisDatabase);

        var actual = await sut.GetNewestRecordAsync(walletId, snapshotId);

        Assert.Null(actual);
    }

    [Fact]
    public async Task 取得最新交易記錄_如果Redis有記錄會使用Redis的最新記錄()
    {
        var walletId = Guid.NewGuid();
        var snapshotId = Guid.Parse("018c3e61-4bfa-0000-0001-07e731f8bf36");

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        await dbContext.Transactions.AddRangeAsync(
            new Transaction
            {
                Id = Guid.Parse("018c3e6e-28c0-0000-0001-07e731f8bf36"),
                Type = (short)TransactionType.Snapshot,
                Balance = 11111M,
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow,
                WalletId = walletId
            },
            new Transaction
            {
                Id = Guid.Parse("018c3e57-124e-0000-0001-07e731f8bf36"),
                Type = (short)TransactionType.Snapshot,
                Balance = 333M,
                PlayerCode = "test1",
                RecordedOn = DateTime.UtcNow,
                WalletId = walletId
            });

        _ = await dbContext.SaveChangesAsync();

        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new WalletRepository(
            dbContext,
            fakeRedisDatabase);

        _ = fakeRedisDatabase.ListGetByIndexAsync(Arg.Is(new RedisKey($"Tgs:Transactions:{walletId}")), Arg.Is(-1L))
            .Returns(JsonSerializer.Serialize(new TransactionRecord(
                Guid.NewGuid(),
                walletId,
                Guid.NewGuid(),
                "test1",
                0M,
                0M,
                22222M,
                0M,
                TransactionType.Deposit,
                null,
                DateTime.UtcNow,
                null,
                null)));

        var actual = await sut.GetNewestRecordAsync(walletId, snapshotId);

        Assert.NotNull(actual);
        Assert.Equal(22222M, actual.Balance);
    }
}
