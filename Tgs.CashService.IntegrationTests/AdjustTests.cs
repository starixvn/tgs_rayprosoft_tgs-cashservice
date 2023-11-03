using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.Client;
using Tgs.CashServiceWorker;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class AdjustTests
{
    [TestMethod]
    public async Task Company_Adjust_To_Wallet_V2()
    {
        var walletId = Guid.NewGuid();
        var companyAdjustAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;
        var userId = Guid.NewGuid();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services.AddSingleton<WalletExchanger>();

                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
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
                        Arg.Is(TransactionType.AdjustCompany),
                        Arg.Is(companyAdjustAmount),
                        Arg.Is(0M),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Is<Guid?>(userId),
                        Arg.Any<CancellationToken>())
                    .Returns(new ExchangeResult(transactionId, postBalance, 0M));

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

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.AdjustCompanyV2Async(
            walletId,
            1000M,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Bonus_Adjust_To_Wallet_V2()
    {
        var walletId = Guid.NewGuid();
        var bonusAdjustAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;
        var userId = Guid.NewGuid();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services.AddSingleton<WalletExchanger>();

                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
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
                        Arg.Is(TransactionType.AdjustBonus),
                        Arg.Is(bonusAdjustAmount),
                        Arg.Is(0M),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Is<Guid?>(userId),
                        Arg.Any<CancellationToken>())
                    .Returns(new ExchangeResult(transactionId, postBalance, 0M));

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

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.AdjustBonusV2Async(
            walletId,
            1000M,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Agent_Adjust_To_Wallet_V2()
    {
        var walletId = Guid.NewGuid();
        var AgentAdjustAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;
        var userId = Guid.NewGuid();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services
                    .AddHttpClientLoggingHandler()
                    .AddSingleton<WalletExchanger>();

                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
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
                        Arg.Is(TransactionType.AdjustAgent),
                        Arg.Is(AgentAdjustAmount),
                        Arg.Is(0M),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Is<Guid?>(userId),
                        Arg.Any<CancellationToken>())
                    .Returns(new ExchangeResult(transactionId, postBalance, 0M));

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

        var httpClient = web.CreateDefaultClient(web.Services.ResolveLoggingHandler<CashServiceClient>());

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.AdjustAgentV2Async(
            walletId,
            1000M,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task System_Adjust_To_Wallet_V2()
    {
        var walletId = Guid.NewGuid();
        var systemAdjustAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;
        var userId = Guid.NewGuid();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services.AddSingleton<WalletExchanger>();

                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
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
                        Arg.Is(TransactionType.AdjustSystem),
                        Arg.Is(systemAdjustAmount),
                        Arg.Is(0M),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Is<Guid?>(userId),
                        Arg.Any<CancellationToken>())
                    .Returns(callInfo =>
                    {
                        var desc = callInfo.ArgAt<string>(6);

                        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(desc));

                        return JsonDocument.TryParseValue(ref reader, out _)
                            ? new ExchangeResult(transactionId, postBalance, 0M)
                            : new ExchangeResult(Guid.Empty, postBalance, 0M);
                    });

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

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.AdjustSystemV2Async(
            walletId,
            1000M,
            null,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task System_Adjust_LockedBalance_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var systemAdjustAmount = -1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;
        var userId = Guid.NewGuid();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services.AddSingleton<WalletExchanger>();

                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
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
                        Arg.Is(TransactionType.AdjustSystem),
                        Arg.Is(0M),
                        Arg.Is(systemAdjustAmount),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Is<Guid?>(userId),
                        Arg.Any<CancellationToken>())
                    .Returns(new ExchangeResult(transactionId, postBalance, 0M));

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

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.AdjustSystemV2Async(
            walletId,
            0M,
            systemAdjustAmount,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Transfer_Adjust_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transferAdjustAmount = 1000M;
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;
        var userId = Guid.NewGuid();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services.AddSingleton<WalletExchanger>();

                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
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
                        Arg.Is(TransactionType.AdjustTransfer),
                        Arg.Is(transferAdjustAmount),
                        Arg.Is(0M),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Is<Guid?>(userId),
                        Arg.Any<CancellationToken>())
                    .Returns(new ExchangeResult(transactionId, postBalance, 0M));

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

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.AdjustTransferAsync(
            walletId,
            1000M,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }
}
