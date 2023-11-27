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
}
