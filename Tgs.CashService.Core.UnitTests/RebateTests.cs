using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class RebateTests
{
    [Test]
    public async Task Can_Create_Rebate_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new RebateCommandService(fakeWalletExchangeService);

        var command = new RebateCommand(
            Guid.NewGuid(),
            100M,
            "新增玩家返水",
            Guid.NewGuid().ToString("N"));

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
    public async Task Can_Create_Dividend_All_Agent_Rebate_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new DividendAllAgentRebateCommandService(fakeWalletExchangeService);

        var command = new DividendAllAgentRebateCommand(
            Guid.NewGuid(),
            100M,
            "新增玩家全民代理返水",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.DividendAllAgentRebate.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Rebate_Truncate_To_2_Decimal_places()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new RebateCommandService(fakeWalletExchangeService);

        var command = new RebateCommand(
            Guid.NewGuid(),
            100.123456789M,
            "新增玩家返水",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Rebate.ToString()
                    && request.Amount == 100.12M
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task All_Agent_Rebate_Truncate_To_2_Decimal_places()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new DividendAllAgentRebateCommandService(fakeWalletExchangeService);

        var command = new DividendAllAgentRebateCommand(
            Guid.NewGuid(),
            100.123456789M,
            "新增玩家全民代理返水",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.DividendAllAgentRebate.ToString()
                    && request.Amount == 100.12M
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }
}
