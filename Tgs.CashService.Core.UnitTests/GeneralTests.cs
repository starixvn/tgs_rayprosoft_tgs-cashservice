using NUnit.Framework.Legacy;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Core.Entities;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class GeneralTests
{
    [Test]
    public async Task Can_Get_Balance()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        
        var sut = new GetBalanceQueryService(fakeTgsRepository);

        var query = new GetBalanceQuery(Guid.NewGuid());
        var wallet = new Wallet { Balance = 1000M };
        fakeTgsRepository.GetWalletByIdAsync(Arg.Is(query.WalletId)).Returns(wallet);

        var actual = await sut.GetAsync(query);

        ClassicAssert.AreEqual(1000M, actual);
    }
}
