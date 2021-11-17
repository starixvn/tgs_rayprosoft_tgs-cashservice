using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class ManagementTests
    {
        [Test]
        public async Task Can_Create_Adjust_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();

            var sut = new AdjustService(fakeWalletRepository);

            var query = new AdjustCommand(
                Guid.NewGuid(),
                100M,
                Guid.NewGuid().ToString("N"),
                "測試調整",
                Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

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
                    Arg.Is(TransactionType.Adjust),
                    Arg.Is(query.Amount),
                    Arg.Is(query.Description),
                    Arg.Is(query.ExternalId),
                    Arg.Is(query.CreatedByUserId));
        }

        [Test]
        public async Task Can_Create_Bonus_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var sut = new BonusService(fakeWalletRepository);

            var query = new BonusCommand(
                Guid.NewGuid(),
                100,
                Guid.NewGuid().ToString("N"),
                "測試紅利");

            fakeWalletRepository.GetWalletAsync(Arg.Is(query.WalletId))
                .Returns(new Wallet
                {
                    Id = query.WalletId,
                    BrandId = Guid.NewGuid(),
                    PlayerCode = "Player1"
                });

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository.Received(1)
                .ExchangeAsync(
                    Arg.Any<Guid>(),
                    Arg.Is(query.WalletId),
                    Arg.Any<string>(),
                    Arg.Is(TransactionType.Bonus),
                    Arg.Is(query.Amount),
                    Arg.Is(query.Description),
                    Arg.Is(query.ExternalId));
        }

        [Test]
        public async Task Can_Create_Rebate_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();

            var sut = new RebateService(fakeWalletRepository);

            var query = new RebateCommand(
                Guid.NewGuid(),
                100M,
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

            _ = fakeWalletRepository.Received(1)
                .ExchangeAsync(
                    Arg.Any<Guid>(),
                    Arg.Is(query.WalletId),
                    Arg.Any<string>(),
                    Arg.Is(TransactionType.Rebate),
                    Arg.Is(query.Amount),
                    Arg.Is(query.Description),
                    Arg.Is(query.ExternalId));
        }

        [Test]
        public async Task Can_Create_Other_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();

            var sut = new OtherService(fakeWalletRepository);

            var query = new OtherCommand(
                Guid.NewGuid(),
                100,
                Guid.NewGuid().ToString("N"),
                "測試其他",
                Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

            fakeWalletRepository.GetWalletAsync(Arg.Is(query.WalletId))
                .Returns(new Wallet
                {
                    Id = query.WalletId,
                    BrandId = Guid.NewGuid(),
                    PlayerCode = "Player1"
                });

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository.Received(1)
                .ExchangeAsync(
                    Arg.Any<Guid>(),
                    Arg.Is(query.WalletId),
                    Arg.Any<string>(),
                    Arg.Is(TransactionType.Other),
                    Arg.Is(query.Amount),
                    Arg.Is(query.Description),
                    Arg.Is(query.ExternalId),
                    Arg.Is(query.CreatedByUserId));
        }
    }
}
