using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class PaymentTests
    {
        [Test]
        public async Task Can_Create_Deposit_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();

            var sut = new DepositService(fakeWalletRepository);

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

            var sut = new DepositService(fakeWalletRepository);

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

        [Test]
        public async Task Can_Create_Withdrawal_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();

            var sut = new WithdrawalService(fakeWalletRepository);

            var query = new WithdrawalCommand(
                Guid.NewGuid(),
                100,
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
                    Arg.Is(-query.Amount),
                    Arg.Is(query.Description),
                    Arg.Is(query.ExternalId));
        }

        [Test]
        public async Task Can_Create_CancelWithdrawal_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();

            var sut = new CancelWithdrawalService(fakeWalletRepository);

            var query = new CancelWithdrawalCommand(
                Guid.NewGuid(),
                100M,
                Guid.NewGuid().ToString("N"),
                "測試取消提款單",
                Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

            fakeWalletRepository.GetWalletAsync(Arg.Is(query.WalletId))
                .Returns(new Wallet
                {
                    BrandId = Guid.NewGuid(),
                    Id = query.WalletId,
                    PlayerCode = "Player1"
                });

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    Arg.Any<Guid>(),
                    Arg.Is(query.WalletId),
                    Arg.Any<string>(),
                    Arg.Is(TransactionType.CancelWithdrawal),
                    Arg.Is(query.Amount),
                    Arg.Is(query.Description),
                    Arg.Is(query.ExternalId),
                    Arg.Is(query.CreatedByUserId));
        }
    }
}
