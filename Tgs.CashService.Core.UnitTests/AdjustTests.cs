using NSubstitute;
using NUnit.Framework;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class AdjustTests
{
    [Test]
    public async Task Can_Create_Company_Adjust_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new CompanyAdjustCommandService(fakeWalletExchangeService);

        var command = new CompanyAdjustCommand(
            Guid.NewGuid(),
            100M,
            "新增公司調帳",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.CompanyAdjust.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_Agent_Adjust_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new AgentAdjustCommandService(fakeWalletExchangeService);

        var command = new AgentAdjustCommand(
            Guid.NewGuid(),
            100M,
            "新增代理調帳",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.AgentAdjust.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_System_Adjust_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new SystemAdjustCommandService(fakeWalletExchangeService);

        var command = new SystemAdjustCommand(
            Guid.NewGuid(),
            100M,
            "新增系統調帳",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.SystemAdjust.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_Transfer_Adjust_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new TransferAdjustCommandService(fakeWalletExchangeService);

        var command = new TransferAdjustCommand(
            Guid.NewGuid(),
            100M,
            "新增系統轉帳",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.TransferAdjust.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }
}