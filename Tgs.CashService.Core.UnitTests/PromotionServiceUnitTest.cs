using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class PromotionServiceUnitTest
    {
        [Test]
        public async Task Can_Create_Promotion_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<PromotionService>.Instance;
            var sut = new PromotionService(faceLogger, fakeWalletRepository);

            var query = new PromotionQuery()
            {
                BrandId = Guid.NewGuid(),
                WalletId = Guid.NewGuid(),
                PlayerCode = "4ucmqplxb029",
                Amount = 100,
                ExternalId = Guid.NewGuid().ToString("N"),
                Description = "測試活動紅利",
            };

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    query.BrandId,
                    query.WalletId,
                    query.PlayerCode,
                    TransactionTypes.Promotion,
                    query.Amount,
                    query.Description,
                    query.ExternalId
                    );
        }
    }
}
