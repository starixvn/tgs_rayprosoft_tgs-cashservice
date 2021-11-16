using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class BonusServiceUnitTest
    {
        [Test]
        public async Task Can_Create_Bonus_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<BonusService>.Instance;
            var sut = new BonusService(faceLogger, fakeWalletRepository);

            var query = new BonusCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "4ucmqplxb029",
                100,
                Guid.NewGuid().ToString("N"),
                "測試紅利");

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    query.BrandId,
                    query.WalletId,
                    query.PlayerCode,
                    TransactionTypes.Bonus,
                    query.Amount,
                    query.Description,
                    query.ExternalId
                    );
        }
    }
}
