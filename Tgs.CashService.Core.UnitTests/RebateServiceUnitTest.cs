using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class RebateServiceUnitTest
    {
        [Test]
        public async Task Can_Create_Rebate_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<RebateService>.Instance;
            var sut = new RebateService(faceLogger, fakeWalletRepository);

            var query = new RebateCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "4ucmqplxb029",
                100,
                Guid.NewGuid().ToString("N"),
                "測試反水");

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    query.BrandId,
                    query.WalletId,
                    query.PlayerCode,
                    TransactionTypes.Rebate,
                    query.Amount,
                    query.Description,
                    query.ExternalId
                    );
        }
    }
}
