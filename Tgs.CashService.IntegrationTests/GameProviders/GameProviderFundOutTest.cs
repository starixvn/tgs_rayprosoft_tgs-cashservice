using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.Client;
using Tgs.CashServiceWorker;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashService.IntegrationTests.GameProviders;

[TestClass]
public class GameProviderFundOutTest
{
    [TestMethod]
    public async Task 整合測試_遊戲轉出()
    {
        var walletId = Guid.NewGuid();
        var fundOutAmount = 1000M;
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

        var actual = await sut.GameProviderFundOutAsync(
            walletId,
            fundOutAmount,
            null,
            null);

        Assert.IsNotNull(actual);
        Assert.AreEqual(0M, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task 整合測試_遊戲轉出_餘額不足()
    {
        var walletId = Guid.NewGuid();
        var fundOutAmount = 1000M;
        var transactionId = Guid.NewGuid();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services
                    .AddSingleton(fakeSerialNumberGenerator)
                    .AddSingleton<WalletExchanger>();

                var wallet = new Wallet(
                    walletId,
                    Guid.NewGuid(),
                    "Player1",
                    100M,
                    0M,
                    Guid.NewGuid());
                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns(_ => wallet);
                _ = fakeWalletRepository.GetBalanceAsync(Arg.Is(walletId), Arg.Any<CancellationToken>())
                    .Returns(_ => wallet.Balance);

                _ = services.AddSingleton(fakeWalletRepository);

                var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

                _ = services.AddSingleton(sp =>
                {
                    var exchanger = sp.GetRequiredService<WalletExchanger>();

                    _ = fakeWalletExchangeService.SendRequestAsync(Arg.Any<WalletExchangeRequest>(), Arg.Any<CancellationToken>())
                        .Returns(callInfo =>
                        {
                            var req = callInfo.Arg<WalletExchangeRequest>();

                            var result = exchanger.RequestAsync(new ExchangeRequest(
                                req.WalletId,
                                Enum.Parse<TransactionType>(req.TransactionType),
                                req.Amount,
                                req.LockAmount,
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;

                            wallet = wallet with
                            {
                                Balance = result.Balance,
                                LockedBalance = result.LockedBalance,
                                SnapshotId = result.TransactionId
                            };

                            return result;
                        });

                    _ = fakeWalletExchangeService.GetBalanceAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                       .Returns(callInfo =>
                       {
                           var walletId = callInfo.Arg<Guid>();

                           return fakeWalletRepository.GetBalanceAsync(walletId);
                       });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        _ = fakeSerialNumberGenerator.GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(transactionId);

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        _ = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => sut.GameProviderFundOutAsync(
            walletId,
            fundOutAmount,
            null,
            null).AsTask());

        var actual = await sut.GetBalanceAsync(walletId);

        Assert.AreEqual(100M, actual);
    }
}
