using NSubstitute;
using NUnit.Framework;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class GameProviderTests
{
    [Test]
    public async Task Can_Create_GameProviderDeposit_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderDepositCommandService(fakeWalletExchangeService);

        var command = new GameProviderDepositCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試遊戲上分");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.GameProviderDeposit.ToString()
                    && request.Amount == -command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderFundIn_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderFundInCommandService(fakeWalletExchangeService);

        var command = new GameProviderFundInCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試轉入主錢包");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.GameProviderFundIn.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderFundOut_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderFundOutCommandService(fakeWalletExchangeService);

        var command = new GameProviderFundOutCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試轉出主錢包");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.GameProviderFundOut.ToString()
                    && request.Amount == -command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderWithdrawal_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderWithdrawalCommandService(fakeWalletExchangeService);

        var command = new GameProviderWithdrawalCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試遊戲下分");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.GameProviderWithdrawal.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderPlaceBet_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderPlaceBetCommandService(fakeWalletExchangeService);

        var command = new GameProviderPlaceBetCommand(
            Guid.NewGuid(),
            100M,
            "測試遊戲下注",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.PlaceBet.ToString()
                           && request.Amount == -command.Amount
                           && request.Description == command.Description
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderSettleWin_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderSettleWinCommandService(fakeWalletExchangeService);

        var command = new GameProviderSettleWinCommand(
            Guid.NewGuid(),
            100M,
            "測試遊戲結算贏",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.WinBet.ToString()
                           && request.Amount == command.Amount
                           && request.Description == command.Description
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderSettleLose_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderSettleLoseCommandService(fakeWalletExchangeService);

        var command = new GameProviderSettleLoseCommand(
            Guid.NewGuid(),
            100M,
            "測試遊戲結算輸",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.LoseBet.ToString()
                           && request.Amount == -command.Amount
                           && request.Description == command.Description
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderSettleTie_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderSettleTieCommandService(fakeWalletExchangeService);

        var command = new GameProviderSettleTieCommand(
            Guid.NewGuid(),
            100M,
            "測試遊戲結算和",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.TieBet.ToString()
                           && request.Amount == command.Amount
                           && request.Description == command.Description
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderSettleFree_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderSettleFreeCommandService(fakeWalletExchangeService);

        var command = new GameProviderSettleFreeCommand(
            Guid.NewGuid(),
            100M,
            "測試遊戲結算免費投注",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.FreeBet.ToString()
                           && request.Amount == command.Amount
                           && request.Description == command.Description
                           && request.ExternalId == command.ExternalId));
    }
}
