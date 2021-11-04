using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class OtherServiceUnitTest
    {
        [Test]
        public async Task Can_Create_Other_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<OtherService>.Instance;
            var sut = new OtherService(faceLogger, fakeWalletRepository);

            var query = new OtherQuery()
            {
                BrandId = Guid.NewGuid(),
                WalletId = Guid.NewGuid(),
                PlayerCode = "4ucmqplxb029",
                Amount = 100,
                ExternalId = Guid.NewGuid().ToString("N"),
                Description = "測試其他",
                CreatedByUserId = Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b")
            };

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    query.BrandId,
                    query.WalletId,
                    query.PlayerCode,
                    TransactionTypes.Other,
                    query.Amount,
                    query.Description,
                    query.ExternalId,
                    query.CreatedByUserId
                    );
        }
    }
}
