using NSubstitute;
using NUnit.Framework;
using Tgs.Cash.Protos;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class ManagementTests
{
    [Test]
    public async Task Can_Create_Adjust_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new AdjustCommandService(fakeWalletRepository, fakeWalletExchangeService);

        var command = new AdjustCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試調整",
            Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Adjust.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_Bonus_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new BonusCommandService(fakeWalletRepository, fakeWalletExchangeService);

        var command = new BonusCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試紅利");

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Bonus.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_Rebate_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new RebateCommandService(fakeWalletRepository, fakeWalletExchangeService);

        var command = new RebateCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試反水");

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Rebate.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_Other_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new OtherCommandService(fakeWalletRepository, fakeWalletExchangeService);

        var command = new OtherCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試其他",
            Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Other.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_UnsettledBetManualCommand_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new UnsettledBetManualCommandService(fakeWalletRepository, fakeWalletExchangeService);

        var command = new UnsettledBetManualCommand(
            Guid.NewGuid(),
            100,
            "測試Open Round Page",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.UnsettledBetManual.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_AdjustResettledBetManualCommand_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new AdjustResettledBetManualCommandService(fakeWalletRepository, fakeWalletExchangeService);

        var command = new AdjustResettledBetManualCommand(
            Guid.NewGuid(),
            100,
            "測試Closed Round Page",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.AdjustResettledBetManual.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }
}
