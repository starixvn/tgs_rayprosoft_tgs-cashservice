using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class GameProviderDepositServiceUnitTest
    {
        [Test]
        public async Task Can_Create_GameProviderDeposit_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<GameProviderDepositService>.Instance;
            var sut = new GameProviderDepositService(faceLogger, fakeWalletRepository);

            var query = new GameProviderDepositQuery()
            {
                BrandId = Guid.NewGuid(),
                WalletId = Guid.NewGuid(),
                PlayerCode = "4ucmqplxb029",
                Amount = -100,
                ExternalId = Guid.NewGuid().ToString("N"),
                Description = "測試遊戲上分",
            };

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    query.BrandId,
                    query.WalletId,
                    query.PlayerCode,
                    TransactionTypes.GameProviderDeposit,
                    query.Amount,
                    query.Description,
                    query.ExternalId
                    );
        }
    }
}
