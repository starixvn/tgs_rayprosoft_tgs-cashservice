using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class GameProviderTests
{
    [Test]
    public async Task Can_Create_GameProviderDeposit_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();

        var sut = new GameProviderDepositCommandService(fakeWalletRepository);

        var query = new GameProviderDepositCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試遊戲上分");

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(query.WalletId))
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
                Arg.Is(TransactionType.GameProviderDeposit),
                Arg.Is(-query.Amount),
                Arg.Is(query.Description),
                Arg.Is(query.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderFundIn_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();

        var sut = new GameProviderFundInCommandService(fakeWalletRepository);

        var query = new GameProviderFundInCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試轉入主錢包");

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(query.WalletId))
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
                Arg.Is(TransactionType.GameProviderFundIn),
                Arg.Is(query.Amount),
                Arg.Is(query.Description),
                Arg.Is(query.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderFundOut_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();

        var sut = new GameProviderFundOutCommandService(fakeWalletRepository);

        var command = new GameProviderFundOutCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試轉出主錢包");

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletRepository
            .Received(1)
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(command.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.GameProviderFundOut),
                Arg.Is(-command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderWithdrawal_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();

        var sut = new GameProviderWithdrawalCommandService(fakeWalletRepository);

        var command = new GameProviderWithdrawalCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試遊戲下分");

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletRepository
            .Received(1)
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(command.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.GameProviderWithdrawal),
                Arg.Is(command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId));
    }
}
