using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class GameProviderFundOutServiceUnitTest
    {
        [Test]
        public async Task Can_Create_GameProviderFundOut_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<GameProviderFundOutService>.Instance;
            var sut = new GameProviderFundOutService(faceLogger, fakeWalletRepository);

            var query = new GameProviderFundOutCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "4ucmqplxb029",
                -100,
                Guid.NewGuid().ToString("N"),
                "測試轉出主錢包");

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    query.BrandId,
                    query.WalletId,
                    query.PlayerCode,
                    TransactionTypes.GameProviderFundOut,
                    query.Amount,
                    query.Description,
                    query.ExternalId
                    );
        }
    }
}
