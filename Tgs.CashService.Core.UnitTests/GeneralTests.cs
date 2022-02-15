using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class GeneralTests
{
    [Test]
    public async Task Can_Get_Balance()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GetBalanceQueryService(fakeWalletExchangeService);

        var query = new GetBalanceQuery(Guid.NewGuid());

        _ = fakeWalletExchangeService.GetBalanceAsync(Arg.Is(query.WalletId))
            .Returns(1000M);

        var actual = await sut.GetAsync(query);

        Assert.AreEqual(1000M, actual);
    }
}
