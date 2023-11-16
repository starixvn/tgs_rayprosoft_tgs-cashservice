using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.Client;
using Tgs.CashServiceWorker;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class PromotionTests
{
    [TestMethod]
    public async Task Promotion_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services
                    .AddSingleton(fakeSerialNumberGenerator)
                    .AddSingleton<WalletExchanger>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns(new Wallet(
                        walletId,
                        Guid.NewGuid(),
                        "Player1",
                        0M,
                        0M,
                        Guid.NewGuid()));

                _ = services.AddSingleton(fakeWalletRepository);

                var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

                _ = services.AddSingleton(sp =>
                {
                    var exchanger = sp.GetRequiredService<WalletExchanger>();

                    _ = fakeWalletExchangeService.SendRequestAsync(Arg.Any<WalletExchangeRequest>(), Arg.Any<CancellationToken>())
                        .Returns(callInfo =>
                        {
                            var req = callInfo.Arg<WalletExchangeRequest>();

                            return exchanger.RequestAsync(new ExchangeRequest(
                                req.WalletId,
                                Enum.Parse<TransactionType>(req.TransactionType),
                                req.Amount,
                                req.LockAmount,
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        _ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(transactionId);

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.PromotionAsync(
            walletId,
            1000M,
            null,
            null,
            null);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Promotion_VipLevel_Upgrade_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services
                    .AddSingleton(fakeSerialNumberGenerator)
                    .AddSingleton<WalletExchanger>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns(new Wallet(
                        walletId,
                        Guid.NewGuid(),
                        "Player1",
                        0M,
                        0M,
                        Guid.NewGuid()));

                _ = services.AddSingleton(fakeWalletRepository);

                var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

                _ = services.AddSingleton(sp =>
                {
                    var exchanger = sp.GetRequiredService<WalletExchanger>();

                    _ = fakeWalletExchangeService.SendRequestAsync(Arg.Any<WalletExchangeRequest>(), Arg.Any<CancellationToken>())
                        .Returns(callInfo =>
                        {
                            var req = callInfo.Arg<WalletExchangeRequest>();

                            return exchanger.RequestAsync(new ExchangeRequest(
                                req.WalletId,
                                Enum.Parse<TransactionType>(req.TransactionType),
                                req.Amount,
                                req.LockAmount,
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        _ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(transactionId);

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.PromotionVipLevelUpgradeAsync(
            walletId,
            1000M,
            null,
            null);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Promotion_VipLevel_Season_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services
                    .AddSingleton(fakeSerialNumberGenerator)
                    .AddSingleton<WalletExchanger>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns(new Wallet(
                        walletId,
                        Guid.NewGuid(),
                        "Player1",
                        0M,
                        0M,
                        Guid.NewGuid()));

                _ = services.AddSingleton(fakeWalletRepository);

                var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

                _ = services.AddSingleton(sp =>
                {
                    var exchanger = sp.GetRequiredService<WalletExchanger>();

                    _ = fakeWalletExchangeService.SendRequestAsync(Arg.Any<WalletExchangeRequest>(), Arg.Any<CancellationToken>())
                        .Returns(callInfo =>
                        {
                            var req = callInfo.Arg<WalletExchangeRequest>();

                            return exchanger.RequestAsync(new ExchangeRequest(
                                req.WalletId,
                                Enum.Parse<TransactionType>(req.TransactionType),
                                req.Amount,
                                req.LockAmount,
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        _ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(transactionId);

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.PromotionVipLevelSeasonAsync(
            walletId,
            1000M,
            null,
            null);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Promotion_VipLevel_Birthday_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services
                    .AddSingleton(fakeSerialNumberGenerator)
                    .AddSingleton<WalletExchanger>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns(new Wallet(
                        walletId,
                        Guid.NewGuid(),
                        "Player1",
                        0M,
                        0M,
                        Guid.NewGuid()));

                _ = services.AddSingleton(fakeWalletRepository);

                var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

                _ = services.AddSingleton(sp =>
                {
                    var exchanger = sp.GetRequiredService<WalletExchanger>();

                    _ = fakeWalletExchangeService
                        .SendRequestAsync(Arg.Any<WalletExchangeRequest>(), Arg.Any<CancellationToken>())
                        .Returns(callInfo =>
                        {
                            var req = callInfo.Arg<WalletExchangeRequest>();

                            return exchanger.RequestAsync(new ExchangeRequest(
                                req.WalletId,
                                Enum.Parse<TransactionType>(req.TransactionType),
                                req.Amount,
                                req.LockAmount,
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        _ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(transactionId);

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.PromotionVipLevelBirthdayAsync(
            walletId,
            1000M,
            null,
            null);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }
}
