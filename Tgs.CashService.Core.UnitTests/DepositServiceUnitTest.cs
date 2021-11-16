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
                100,
                Guid.NewGuid().ToString("N"),
                "測試存款");

            fakeWalletRepository.GetWalletAsync(Arg.Is(query.WalletId))
                .Returns(new Wallet
                {
                    Id = query.WalletId,
                    BrandId = Guid.NewGuid(),
                    PlayerCode = "Player1"
                });

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    Arg.Any<Guid>(),
                    query.WalletId,
                    Arg.Any<string>(),
                    TransactionType.Deposit,
                    query.Amount,
                    query.Description,
                    query.ExternalId
                    );
        }

        [Test]
        public void Create_Deposit_Exchange_With_Invalid_WalletId_Will_Throw_Exception()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<DepositService>.Instance;
            var sut = new DepositService(faceLogger, fakeWalletRepository);

            var query = new DepositCommand(
                Guid.NewGuid(),
                100,
                Guid.NewGuid().ToString("N"),
                "測試存款");

            Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.GetAsync(query));

            _ = fakeWalletRepository.DidNotReceive()
                .ExchangeAsync(
                    Arg.Any<Guid>(),
                    query.WalletId,
                    Arg.Any<string>(),
                    TransactionType.Deposit,
                    query.Amount,
                    query.Description,
                    query.ExternalId);
        }
    }
}
