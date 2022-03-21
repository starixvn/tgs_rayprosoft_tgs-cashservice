using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.Client;
using Tgs.CashServiceWorker;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class GameTests
{
    [TestMethod]
    public async Task Game_PlaceBet_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var betAmount = 1000m;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345m;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                services.AddSingleton<WalletExchanger>();

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
                        Arg.Is(TransactionType.GamePlaceBet),
                        Arg.Is(-betAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns(new ExchangeResult(transactionId, postBalance));

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
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GamePlaceBetAsync(
            walletId,
            betAmount,
            Guid.NewGuid().ToString("N"));

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Game_Settle_Win_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var settleAmount = 1000m;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345m;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                services.AddSingleton<WalletExchanger>();

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
                        Arg.Is(TransactionType.GameWinBet),
                        Arg.Is(settleAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns(new ExchangeResult(transactionId, postBalance));

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
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GameSettleWinAsync(
            walletId,
            settleAmount,
            Guid.NewGuid().ToString("N"));

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Game_Settle_Lose_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var settleAmount = 0m;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345m;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                services.AddSingleton<WalletExchanger>();

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
                        Arg.Is(TransactionType.GameLoseBet),
                        Arg.Is(-settleAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns(new ExchangeResult(transactionId, postBalance));

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
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GameSettleLoseAsync(
            walletId,
            settleAmount,
            Guid.NewGuid().ToString("N"));

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Game_Settle_Tie_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var settleAmount = 1000m;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345m;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                services.AddSingleton<WalletExchanger>();

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
                        Arg.Is(TransactionType.GameTieBet),
                        Arg.Is(settleAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns(new ExchangeResult(transactionId, postBalance));

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
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GameSettleTieAsync(
            walletId,
            settleAmount,
            Guid.NewGuid().ToString("N"));

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Game_Settle_Free_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var settleAmount = 1000m;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345m;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                services.AddSingleton<WalletExchanger>();

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
                        Arg.Is(TransactionType.GameFreeBet),
                        Arg.Is(settleAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns(new ExchangeResult(transactionId, postBalance));

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
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GameSettleFreeAsync(
            walletId,
            settleAmount,
            Guid.NewGuid().ToString("N"));

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Game_Cancel_PlaceBet_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var cancelAmount = 1000m;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345m;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                services.AddSingleton<WalletExchanger>();

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
                        Arg.Is(TransactionType.GameCancelTransaction),
                        Arg.Is(cancelAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns(new ExchangeResult(transactionId, postBalance));

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
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GameCancelPlaceBetAsync(
            walletId,
            cancelAmount,
            Guid.NewGuid().ToString("N"));

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Game_Cancel_Settlement_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var cancelAmount = 1000m;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345m;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                services.AddSingleton<WalletExchanger>();

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
                        Arg.Is(TransactionType.GameCancelTransaction),
                        Arg.Is(-cancelAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns(new ExchangeResult(transactionId, postBalance));

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
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GameCancelSettlementAsync(
            walletId,
            cancelAmount,
            Guid.NewGuid().ToString("N"));

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Game_LockAmount_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var lockAmount = 1000m;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345m;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                services.AddSingleton<WalletExchanger>();

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
                        Arg.Is(TransactionType.GameLockAmount),
                        Arg.Is(-lockAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns(new ExchangeResult(transactionId, postBalance));

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
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GameLockAmountAsync(
            walletId,
            lockAmount,
            Guid.NewGuid().ToString("N"));

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Game_UnlockAmount_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var unlockAmount = 1000m;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345m;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                services.AddSingleton<WalletExchanger>();

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
                        Arg.Is(TransactionType.GameUnlockAmount),
                        Arg.Is(unlockAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns(new ExchangeResult(transactionId, postBalance));

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
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GameUnlockAmountAsync(
            walletId,
            unlockAmount,
            Guid.NewGuid().ToString("N"));

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }
}
