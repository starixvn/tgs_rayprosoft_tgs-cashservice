using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class AdjustServiceUnitTest
    {
        [Test]
        public async Task Can_Create_Adjust_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<AdjustService>.Instance;
            var sut = new AdjustService(faceLogger, fakeWalletRepository);

            var query = new AdjustCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "4ucmqplxb029",
                100,
                Guid.NewGuid().ToString("N"),
                "´ú¸Õ½Õ¾ã",
                Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    query.BrandId,
                    query.WalletId,
                    query.PlayerCode,
                    TransactionType.Adjust,
                    query.Amount,
                    query.Description,
                    query.ExternalId,
                    query.CreatedByUserId
                    );
        }
    }
}
