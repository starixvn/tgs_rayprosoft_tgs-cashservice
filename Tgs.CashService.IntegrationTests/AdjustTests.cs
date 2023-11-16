using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.Client;
using Tgs.CashServiceWorker;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class AdjustTests
{
    [TestMethod]
    public async Task Company_Adjust_To_Wallet_V2()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

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

        var actual = await sut.AdjustCompanyV2Async(
            walletId,
            1000M,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Bonus_Adjust_To_Wallet_V2()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

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

        var actual = await sut.AdjustBonusV2Async(
            walletId,
            1000M,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Agent_Adjust_To_Wallet_V2()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services
                    .AddSingleton(fakeSerialNumberGenerator)
                    .AddHttpClientLoggingHandler()
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

        var httpClient = web.CreateDefaultClient(web.Services.ResolveLoggingHandler<CashServiceClient>());

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.AdjustAgentV2Async(
            walletId,
            1000M,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task System_Adjust_To_Wallet_V2()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

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

        var actual = await sut.AdjustSystemV2Async(
            walletId,
            1000M,
            null,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);

        _ = fakeWalletRepository.Received(1)
            .WriteTransactionRecordAsync(
                Arg.Is<TransactionRecord>(record => record.Type == TransactionType.AdjustSystem),
                Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task System_Adjust_LockedBalance_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var systemAdjustAmount = -1000M;
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

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
                        1000M,
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

        var actual = await sut.AdjustSystemV2Async(
            walletId,
            0M,
            systemAdjustAmount,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(0M, actual.Balance);
        Assert.AreEqual(0M, actual.LockedBalance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Transfer_Adjust_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

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

        var actual = await sut.AdjustTransferAsync(
            walletId,
            1000M,
            null,
            null,
            userId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }
}
