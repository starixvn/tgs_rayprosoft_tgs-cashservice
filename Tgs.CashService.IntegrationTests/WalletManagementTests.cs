using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tgs.CashService.Client;
using Tgs.CashService.Repositories.Entities;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class WalletManagementTests
{
    [TestMethod]
    public async Task 建立新錢包()
    {
        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services => services
                .AddFakeNatsMessageQueue()
                .AddHttpClientLoggingHandler()));

        var httpClient = web.CreateDefaultClient(web.Services.ResolveLoggingHandler<CashServiceClient>());

        var sut = new CashServiceClient(httpClient);
        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var userId = "test1";
        var currencyCode = "CNY";
        var brandId = Guid.NewGuid();

        var actual = await sut.CreateNewCashAsync(brandId, userId, currencyCode, CashType.Agent);
        var insertedRow = await dbContext.Wallets.FirstOrDefaultAsync();

        Assert.IsNotNull(insertedRow);
        Assert.AreEqual(actual, insertedRow.Id);
        Assert.AreEqual(userId, insertedRow.PlayerCode);
        Assert.AreEqual(currencyCode, insertedRow.CurrencyCode);
        Assert.AreEqual(0M, insertedRow.Balance);
        Assert.AreEqual(0M, insertedRow.LockedBalance);
        Assert.AreEqual(brandId, insertedRow.BrandId);
        Assert.AreEqual(nameof(CashType.Agent), insertedRow.CashType);
    }
}
