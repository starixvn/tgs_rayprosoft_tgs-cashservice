using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.CashService.Core;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class PromotionTests
{
    [TestMethod]
    public async Task Promotion_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var promotionAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = new TestCashApplication(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.Promotion),
                        Arg.Is(promotionAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns((transactionId, postBalance));

                services.AddSingleton(fakeWalletRepository);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.PromotionAsync(
            walletId,
            1000M,
            null,
            null,
            null);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Promotion_VipLevel_Upgrade_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var promotionAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = new TestCashApplication(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.PromotionVipLevelUpgrade),
                        Arg.Is(promotionAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns((transactionId, postBalance));

                services.AddSingleton(fakeWalletRepository);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.PromotionVipLevelUpgradeAsync(
            walletId,
            1000M,
            null,
            null);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Promotion_VipLevel_Season_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var promotionAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = new TestCashApplication(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.PromotionVipLevelSeason),
                        Arg.Is(promotionAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns((transactionId, postBalance));

                services.AddSingleton(fakeWalletRepository);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.PromotionVipLevelSeasonAsync(
            walletId,
            1000M,
            null,
            null);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }
}
