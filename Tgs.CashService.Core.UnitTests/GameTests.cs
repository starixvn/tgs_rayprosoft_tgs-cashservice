using NSubstitute;
using NUnit.Framework;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class GameTests
{
    [Test]
    public async Task Can_Create_GamePlaceBet_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GamePlaceBetCommandService(fakeWalletExchangeService);

        var command = new GamePlaceBetCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GamePlaceBet.ToString()
                           && request.Amount == -command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameSettleWin_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameSettleWinCommandService(fakeWalletExchangeService);

        var command = new GameSettleWinCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameWinBet.ToString()
                           && request.Amount == command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameSettleLose_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameSettleLoseCommandService(fakeWalletExchangeService);

        var command = new GameSettleLoseCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameLoseBet.ToString()
                           && request.Amount == -command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameSettleTie_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameSettleTieCommandService(fakeWalletExchangeService);

        var command = new GameSettleTieCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameTieBet.ToString()
                           && request.Amount == command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameSettleFree_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameSettleFreeCommandService(fakeWalletExchangeService);

        var command = new GameSettleFreeCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameFreeBet.ToString()
                           && request.Amount == command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameCancelPlaceBet_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameCancelPlaceBetCommandService(fakeWalletExchangeService);

        var command = new GameCancelPlaceBetCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameCancelTransaction.ToString()
                           && request.Amount == command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameCancelSettlement_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameCancelSettlementCommandService(fakeWalletExchangeService);

        var command = new GameCancelSettlementCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameCancelTransaction.ToString()
                           && request.Amount == -command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameLockAmount_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameLockAmountCommandService(fakeWalletExchangeService);

        var command = new GameLockAmountCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameLockAmount.ToString()
                           && request.Amount == -command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameUnlockAmount_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameUnlockAmountCommandService(fakeWalletExchangeService);

        var command = new GameUnlockAmountCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameUnlockAmount.ToString()
                           && request.Amount == command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }
}
