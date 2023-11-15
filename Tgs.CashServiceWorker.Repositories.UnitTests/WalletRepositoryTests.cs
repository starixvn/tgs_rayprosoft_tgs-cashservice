using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.CashService;
using Tgs.CashService.Repositories;
using Tgs.SerialNumberGenerator;
using TGS.GameProviders.Repositories.Entities;

namespace Tgs.CashServiceWorker.Repositories.UnitTests;

public class WalletRepositoryTests
{
    [Fact]
    public async Task 錢包操作資料庫動作()
    {
        var brandId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var transactionType = TransactionType.WithdrawCommit;
        var description = "test";
        var externalId = Guid.NewGuid().ToString("N");
        var createdByUserId = Guid.NewGuid();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        _ = dbContext.WalletItems.Add(new WalletItem
        {
            Id = walletId,
            Balance = 0M,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            PlayerCode = "tester",
            CashType = nameof(CashType.Player)
        });

        _ = await dbContext.SaveChangesAsync();

        var fakeDbScriptExecutor = Substitute.For<IDbScriptExecutor>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var sut = new WalletRepository(
            dbContext,
            fakeDbScriptExecutor,
            fakeSerialNumberGenerator,
            NullLogger<WalletRepository>.Instance);

        _ = fakeDbScriptExecutor.QueryFirstOrDefaultAsync<WalletBalanceReturn>(Arg.Any<string>(), Arg.Any<object>())
            .Returns(new WalletBalanceReturn
            {
                OriginBalance = 100M,
                OriginLockedBalance = 100M,
                PostBalance = 200M,
                PostLockedBalance = 20M
            });

        _ = fakeSerialNumberGenerator.GenerateAsync()
            .Returns(Guid.NewGuid());

        var actual = await sut.ExchangeAsync(
           brandId,
           walletId,
           "tester",
           transactionType,
           100M,
           -80M,
           description,
           externalId,
           createdByUserId);

        var transLog = await dbContext.Transactions.FirstAsync();

        Assert.Equal(200M, actual.Balance);
        Assert.Equal(20M, actual.LockedBalance);

        Assert.Equal(200M, transLog.Balance);
        Assert.Equal(20M, transLog.LockedBalance);
        Assert.Equal(100M, transLog.Amount);
        Assert.Equal(-80M, transLog.LockAmount);
    }

    [Fact]
    public async Task 錢包操作後_如果PostBalance餘額小於0_操作會失敗()
    {
        var brandId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var transactionType = TransactionType.WithdrawCommit;
        var description = "test";
        var externalId = Guid.NewGuid().ToString("N");
        var createdByUserId = Guid.NewGuid();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        _ = dbContext.WalletItems.Add(new WalletItem
        {
            Id = walletId,
            Balance = 0M,
            CurrencyCode = "CNY",
            ExternalCurrencyCode = "CNY",
            PlayerCode = "tester",
            CashType = nameof(CashType.Player)
        });

        _ = await dbContext.SaveChangesAsync();

        var fakeDbScriptExecutor = Substitute.For<IDbScriptExecutor>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var sut = new WalletRepository(
            dbContext,
            fakeDbScriptExecutor,
            fakeSerialNumberGenerator,
            NullLogger<WalletRepository>.Instance);

        _ = fakeDbScriptExecutor.QueryFirstOrDefaultAsync<WalletBalanceReturn>(Arg.Any<string>(), Arg.Any<object>())
            .Returns(new WalletBalanceReturn
            {
                OriginBalance = 100M,
                OriginLockedBalance = 100M,
                PostBalance = 200M,
                PostLockedBalance = -20M
            });

        _ = fakeSerialNumberGenerator.GenerateAsync()
            .Returns(Guid.NewGuid());

        _ = Assert.ThrowsAsync<WalletExchangeFailException>(async () => await sut.ExchangeAsync(
           brandId,
           walletId,
           "tester",
           transactionType,
           100M,
           -80M,
           description,
           externalId,
           createdByUserId));

        var transLog = await dbContext.Transactions.FirstOrDefaultAsync();

        Assert.Null(transLog);
    }

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

        var fakeDbScriptExecutor = Substitute.For<IDbScriptExecutor>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var sut = new WalletRepository(
            dbContext,
            fakeDbScriptExecutor,
            fakeSerialNumberGenerator,
            NullLogger<WalletRepository>.Instance);

        var actual = await sut.GetBalanceAsync(walletId);

        Assert.Equal(balance, actual);
    }

    [Fact]
    public async Task 取得錢包資料_如果BrandId欄位是Null_會從玩家資料表查詢()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var fakeDbScriptExecutor = Substitute.For<IDbScriptExecutor>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var sut = new WalletRepository(
            dbContext,
            fakeDbScriptExecutor,
            fakeSerialNumberGenerator,
            NullLogger<WalletRepository>.Instance);

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

        Assert.NotNull(actual);
        Assert.Equal(brandId, actual.BrandId);
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

        var fakeDbScriptExecutor = Substitute.For<IDbScriptExecutor>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var sut = new WalletRepository(
            dbContext,
            fakeDbScriptExecutor,
            fakeSerialNumberGenerator,
            NullLogger<WalletRepository>.Instance);

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
