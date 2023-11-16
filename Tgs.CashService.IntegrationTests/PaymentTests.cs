using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.Client;
using Tgs.CashServiceWorker;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class PaymentTests
{
    [TestMethod]
    [Obsolete("已淘汰")]
    public async Task Deposit_Amount_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(
            builder => builder.ConfigureServices(services =>
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

        var actual = await sut.DepositAsync(
            walletId,
            1000M,
            null,
            null,
            null);

        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Deposited_Amount_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(
            builder => builder.ConfigureServices(services =>
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

        var actual = await sut.DepositedAsync(
            walletId,
            1000M,
            null,
            null,
            null);

        Assert.AreEqual(1000, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    [Obsolete("已淘汰")]
    public async Task Withdraw_Amount_From_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(
            builder => builder.ConfigureServices(services =>
            {
                _ = services
                    .AddSingleton(fakeSerialNumberGenerator)
                    .AddSingleton<WalletExchanger>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns(new Wallet(
                        walletId,
                        Guid.NewGuid(),
                        "Player1",
                        1000M,
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

        var actual = await sut.WithdrawalAsync(
            walletId,
            1000M,
            null,
            null);

        Assert.AreEqual(0M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    [Obsolete("已淘汰")]
    public async Task Cancel_Withdraw_From_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var cancelAmount = 1000M;

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(
            builder => builder.ConfigureServices(services =>
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

        var actual = await sut.CancelWithdrawalAsync(
            walletId,
            cancelAmount,
            null,
            null,
            Guid.NewGuid());

        Assert.AreEqual(cancelAmount, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task LockWithdraw_Amount_From_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(
            builder => builder.ConfigureServices(services =>
            {
                _ = services
                    .AddSingleton(fakeSerialNumberGenerator)
                    .AddSingleton<WalletExchanger>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns(new Wallet(
                        walletId,
                        Guid.NewGuid(),
                        "Player1",
                        1000M,
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

        var actual = await sut.LockWithdrawAsync(
            walletId,
            1000M,
            null,
            null);

        Assert.AreEqual(0M, actual.Balance);
        Assert.AreEqual(1000M, actual.LockedBalance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task RollbackWithdraw_Amount_From_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var amount = 1000M;

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(
            builder => builder.ConfigureServices(services =>
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

        var actual = await sut.RollbackWithdrawAsync(
            walletId,
            amount,
            null,
            null,
            Guid.NewGuid());

        Assert.AreEqual(amount, actual.Balance);
        Assert.AreEqual(0M, actual.LockedBalance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task CommitWithdraw_Amount_From_Wallet()
    {
        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var amount = 1000M;
        var actualAmount = 900M;

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(
            builder => builder.ConfigureServices(services =>
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

        var actual = await sut.CommitWithdrawAsync(
            walletId,
            amount,
            actualAmount,
            null,
            null,
            Guid.NewGuid());

        Assert.AreEqual(0M, actual.Balance);
        Assert.AreEqual(0M, actual.LockedBalance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }
}
