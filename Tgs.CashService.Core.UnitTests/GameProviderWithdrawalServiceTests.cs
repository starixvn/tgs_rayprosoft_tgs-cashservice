using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class GameProviderWithdrawalServiceTests
    {
        [Test]
        public async Task Can_Create_GameProviderWithdrawal_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<GameProviderWithdrawalService>.Instance;
            var sut = new GameProviderWithdrawalService(faceLogger, fakeWalletRepository);

            var query = new GameProviderWithdrawalCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "4ucmqplxb029",
                100,
                Guid.NewGuid().ToString("N"),
                "測試遊戲下分");

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    query.BrandId,
                    query.WalletId,
                    query.PlayerCode,
                    TransactionTypes.GameProviderWithdrawal,
                    query.Amount,
                    query.Description,
                    query.ExternalId
                    );
        }
    }
}
