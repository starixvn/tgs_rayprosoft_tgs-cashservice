using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.SerialNumberGenerator;
using TGS.GameProviders.Repositories.Entities;

namespace Tgs.CashService.Repositories.UnitTests;

[TestFixture]
public class WalletRepositoryUnitTest
{
    [Test]
    public async Task Exchange()
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
            PlayerCode = "tester"
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
           createdByUserId).ConfigureAwait(false);

        var transLog = await dbContext.Transactions.FirstAsync();

        Assert.AreEqual(200M, actual.Balance);
        Assert.AreEqual(20M, actual.LockedBalance);

        Assert.AreEqual(200M, transLog.Balance);
        Assert.AreEqual(20M, transLog.LockedBalance);
        Assert.AreEqual(100M, transLog.Amount);
        Assert.AreEqual(-80M, transLog.LockAmount);
    }

    [Test]
    public async Task Exchange_If_PostLockedBalance_Less_Then_Zero_Will_Fail()
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
            PlayerCode = "tester"
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

        Assert.IsNull(transLog);
    }

    [Test]
    public async Task GetBalance()
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
            PlayerCode = "tester"
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

        Assert.AreEqual(balance, actual);
    }
}
