using Microsoft.Extensions.DependencyInjection;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class DependencyInjectionTests
{
    [TestMethod]
    public async Task 整合測試_WalletExchangeService應該被WalletExchangeServiceDecorator裝飾()
    {
        await using var web = new TestCashApplication();

        var actual = web.Services.GetRequiredService<IWalletExchangeService>();

        Assert.IsInstanceOfType<WalletExchangeServiceDecorator>(actual);
    }
}
