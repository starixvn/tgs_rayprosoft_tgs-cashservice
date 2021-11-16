using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class DepositServiceUnitTest
    {
        [Test]
        public async Task Can_Create_Deposit_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<DepositService>.Instance;
            var sut = new DepositService(faceLogger, fakeWalletRepository);

            var query = new DepositCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "4ucmqplxb029",
                100,
                Guid.NewGuid().ToString("N"),
                "測試存款");

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    query.BrandId,
                    query.WalletId,
                    query.PlayerCode,
                    TransactionTypes.Deposit,
                    query.Amount,
                    query.Description,
                    query.ExternalId
                    );
        }
    }
}
