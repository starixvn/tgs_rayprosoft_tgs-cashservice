using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.CashService.Core;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class GeneralTests
{
    [TestMethod]
    public async Task Can_Get_Balance_By_WalletId()
    {
        var walletId = Guid.NewGuid();
        var expectedAmount = 1000M;

        await using var web = new TestCashApplication(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                fakeWalletRepository.GetBalanceAsync(Arg.Is(walletId))
                    .Returns(expectedAmount);

                services.AddSingleton(fakeWalletRepository);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GetBalanceAsync(walletId);

        Assert.AreEqual(actual, expectedAmount);
    }
}
