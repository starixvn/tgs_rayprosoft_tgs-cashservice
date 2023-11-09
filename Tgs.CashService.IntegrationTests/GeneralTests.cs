using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.Client;
using Tgs.CashServiceWorker;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class GeneralTests
{
    [TestMethod]
    public async Task Can_Get_Balance_By_WalletId()
    {
        var walletId = Guid.NewGuid();
        var expectedAmount = 1000M;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services.AddSingleton<WalletExchanger>();

                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetBalanceAsync(Arg.Is(walletId))
                    .Returns(expectedAmount);

                _ = services.AddSingleton(fakeWalletRepository);

                var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

                _ = services.AddSingleton(sp =>
                {
                    var exchanger = sp.GetRequiredService<WalletExchanger>();

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

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GetBalanceAsync(walletId);

        Assert.AreEqual(actual, expectedAmount);
    }
}
