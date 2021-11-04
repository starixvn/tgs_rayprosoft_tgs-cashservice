using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class BalanceServiceUnitTest
    {
        [Test]
        public async Task Can_Get_Balance()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<BalanceService>.Instance;
            var sut = new BalanceService(faceLogger, fakeWalletRepository);

            var query = new BalanceQuery()
            {
                WalletId = Guid.NewGuid()
            };

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .GetBalanceAsync(query.WalletId);
        }
    }
}
