using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class PromotionTests
{
    [Test]
    public async Task Can_Create_Promotion_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();

        var sut = new PromotionCommandService(fakeWalletRepository);

        var command = new PromotionCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試活動紅利",
            null);

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletRepository.Received(1)
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(command.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.Promotion),
                Arg.Is(command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId));
    }

    [Test]
    public async Task Can_Create_Promotion_VipLevel_Upgrad_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();

        var sut = new PromotionVipLevelupgradeCommandService(fakeWalletRepository);

        var command = new PromotionVipLevelUpgradeCommand(
            Guid.NewGuid(),
            100M,
            "測試VIP等級升級",
            Guid.NewGuid().ToString("N"));

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletRepository.Received(1)
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(command.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.PromotionVipLevelUpgrade),
                Arg.Is(command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId));
    }

    [Test]
    public async Task Can_Create_Promotion_VipLevel_Season_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();

        var sut = new PromotionVipLevelSeasonCommandService(fakeWalletRepository);

        var command = new PromotionVipLevelSeasonCommand(
            Guid.NewGuid(),
            100M,
            "測試VIP季福利",
            Guid.NewGuid().ToString("N"));

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletRepository.Received(1)
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(command.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.PromotionVipLevelSeason),
                Arg.Is(command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId));
    }
}
