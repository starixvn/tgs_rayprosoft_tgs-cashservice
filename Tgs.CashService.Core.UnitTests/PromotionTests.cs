using NSubstitute;
using NUnit.Framework;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class PromotionTests
{
    [Test]
    public async Task Can_Create_Promotion_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new PromotionCommandService(fakeWalletExchangeService);

        var command = new PromotionCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試活動紅利",
            null);

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Promotion.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_Promotion_VipLevel_Upgrad_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new PromotionVipLevelupgradeCommandService(fakeWalletExchangeService);

        var command = new PromotionVipLevelUpgradeCommand(
            Guid.NewGuid(),
            100M,
            "測試VIP等級升級",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.PromotionVipLevelUpgrade.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_Promotion_VipLevel_Season_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new PromotionVipLevelSeasonCommandService(fakeWalletExchangeService);

        var command = new PromotionVipLevelSeasonCommand(
            Guid.NewGuid(),
            100M,
            "測試VIP季福利",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.PromotionVipLevelSeason.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_Promotion_VipLevel_Birthday_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new PromotionVipLevelBirthdayCommandService(fakeWalletExchangeService);

        var command = new PromotionVipLevelBirthdayCommand(
            Guid.NewGuid(),
            100M,
            "測試VIP生日福利",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.PromotionVipLevelBirthday.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Truncate_To_2_Decimal_places()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new PromotionCommandService(fakeWalletExchangeService);

        var command = new PromotionCommand(
            Guid.NewGuid(),
            100.123456M,
            Guid.NewGuid().ToString("N"),
            "測試活動紅利",
            null);

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Promotion.ToString()
                    && request.Amount == 100.12M
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }
}
