using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.CashService.Core;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class ManagementTests
{
    [TestMethod]
    public async Task Adjust_Amount_From_Wallet()
    {
        var walletId = Guid.NewGuid();
        var adjustAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                _ = fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.Adjust),
                        Arg.Is(adjustAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns((transactionId, postBalance));

                _ = services.AddSingleton(fakeWalletRepository);
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.AdjustAsync(
            walletId,
            adjustAmount,
            null,
            null,
            Guid.NewGuid());

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Bonus_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var bonusAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                _ = fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.Bonus),
                        Arg.Is(bonusAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns((transactionId, postBalance));

                _ = services.AddSingleton(fakeWalletRepository);
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.BonusAsync(walletId, 1000M, null, null);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Other_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var otherAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                _ = fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.Other),
                        Arg.Is(otherAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns((transactionId, postBalance));

                _ = services.AddSingleton(fakeWalletRepository);
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.OtherAsync(
            walletId,
            1000M,
            "reason",
            "referenceId",
            Guid.NewGuid());

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Rebate_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var rebateAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                _ = fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.Rebate),
                        Arg.Is(rebateAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns((transactionId, postBalance));

                _ = services.AddSingleton(fakeWalletRepository);
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.RebateAsync(
            walletId,
            1000M,
            null,
            null);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task UnsettledBetManual_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var otherAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                _ = fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.UnsettledBetManual),
                        Arg.Is(otherAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns((transactionId, postBalance));

                _ = services.AddSingleton(fakeWalletRepository);
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.UnsettledBetManualAsync(
            walletId,
            1000M,
            "reason",
            "referenceId",
            Guid.NewGuid());

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }
}
