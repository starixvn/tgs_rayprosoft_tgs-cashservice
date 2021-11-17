using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class GeneralTests
    {
        [Test]
        public async Task Can_Get_Balance()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();

            var sut = new BalanceService(fakeWalletRepository);

            var query = new GetBalanceQuery(Guid.NewGuid());

            fakeWalletRepository.GetBalanceAsync(Arg.Is(query.WalletId))
                .Returns(1000M);

            var actual = await sut.GetAsync(query);

            Assert.AreEqual(1000M, actual);
        }
    }
}
