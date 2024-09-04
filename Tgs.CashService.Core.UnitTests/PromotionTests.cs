using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class PromotionTests
{
    [Test]
    public async Task PromotionCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new PromotionCommandService(fakeTgsRepository);

        var command = new PromotionCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試活動紅利",
            null);

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.Promotion
                  && record.Amount == command.Amount
                  && record.LockAmount == 0
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task PromotionVipLevelupgradeCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new PromotionVipLevelupgradeCommandService(fakeTgsRepository);

        var command = new PromotionVipLevelUpgradeCommand(
            Guid.NewGuid(),
            100M,
            "測試VIP等級升級",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
               .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                   record => record.WalletId == command.WalletId
                   && record.TransactionType == TransactionType.PromotionVipLevelUpgrade
                   && record.Amount == command.Amount
                   && record.LockAmount == 0
                   && record.Description == command.Description
                   && record.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task PromotionVipLevelSeasonCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new PromotionVipLevelSeasonCommandService(fakeTgsRepository);

        var command = new PromotionVipLevelSeasonCommand(
            Guid.NewGuid(),
            100M,
            "測試VIP季福利",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
               .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                   record => record.WalletId == command.WalletId
                   && record.TransactionType == TransactionType.PromotionVipLevelSeason
                   && record.Amount == command.Amount
                   && record.LockAmount == 0
                   && record.Description == command.Description
                   && record.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task PromotionVipLevelBirthdayCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new PromotionVipLevelBirthdayCommandService(fakeTgsRepository);

        var command = new PromotionVipLevelBirthdayCommand(
            Guid.NewGuid(),
            100M,
            "測試VIP生日福利",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.PromotionVipLevelBirthday
                  && record.Amount == command.Amount
                  && record.LockAmount == 0
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task PromotionCommandService_TruncateAmount_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new PromotionCommandService(fakeTgsRepository);

        var command = new PromotionCommand(
            Guid.NewGuid(),
            100.123456M,
            Guid.NewGuid().ToString("N"),
            "測試活動紅利",
            null);

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.Promotion
                  && record.Amount == command.Amount.Truncate(2)
                  && record.LockAmount == 0
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId));
    }
}
