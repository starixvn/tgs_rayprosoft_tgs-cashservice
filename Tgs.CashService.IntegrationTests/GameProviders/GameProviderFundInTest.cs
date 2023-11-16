using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.Client;
using Tgs.CashServiceWorker;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashService.IntegrationTests.GameProviders;

[TestClass]
public class GameProviderFundInTest
{
    [TestMethod]
    public async Task GameProvider_FundIn_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var fundInAmount = 1000M;
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

        var actual = await sut.GameProviderFundInAsync(
            walletId,
            fundInAmount,
            null,
            null);

        Assert.IsNotNull(actual);
        Assert.AreEqual(fundInAmount, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }
}
