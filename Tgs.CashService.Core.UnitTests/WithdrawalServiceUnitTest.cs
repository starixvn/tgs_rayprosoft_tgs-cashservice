using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class WithdrawalServiceUnitTest
    {
        [Test]
        public async Task Can_Create_Withdrawal_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<WithdrawalService>.Instance;
            var sut = new WithdrawalService(faceLogger, fakeWalletRepository);

            var query = new WithdrawalCommand(
                Guid.NewGuid(),
                -100,
                Guid.NewGuid().ToString("N"),
                "測試反水");

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
                    Arg.Is(query.WalletId),
                    Arg.Any<string>(),
                    Arg.Is(TransactionType.Withdrawal),
                    query.Amount,
                    query.Description,
                    query.ExternalId);
        }
    }
}
