using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class RebateTests
{
    [Test]
    public async Task Can_Create_Rebate_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();

        var sut = new RebateCommandService(fakeWalletRepository);

        var command = new RebateCommand(
            Guid.NewGuid(),
            100M,
            "新增玩家返水",
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
                Arg.Is(TransactionType.Rebate),
                Arg.Is(command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId));
    }

    [Test]
    public async Task Can_Create_Dividend_All_Agent_Rebate_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();

        var sut = new DividendAllAgentRebateCommandService(fakeWalletRepository);

        var command = new DividendAllAgentRebateCommand(
            Guid.NewGuid(),
            100M,
            "新增玩家全民代理返水",
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
                Arg.Is(TransactionType.DividendAllAgentRebate),
                Arg.Is(command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId));
    }
}
