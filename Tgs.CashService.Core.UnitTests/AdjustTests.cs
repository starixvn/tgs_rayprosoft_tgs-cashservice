using System.Text.Json;
using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class AdjustTests
{
    [Test]
    public async Task Can_Create_Company_Adjust_Exchange_V2()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new AdjustCompanyCommandService(fakeWalletExchangeService);

        var command = new AdjustCompanyV2Command(
            Guid.NewGuid(),
            100M,
            "新增公司調帳",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        var description = JsonSerializer.Serialize(new
        {
            TransactionType = (int)TransactionType.AdjustCompany,
            Remark = command.Description
        });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.AdjustCompany.ToString()
                    && request.Amount == command.Amount
                    && request.Description == description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_Bonus_Adjust_Exchange_V2()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new AdjustBonusCommandService(fakeWalletExchangeService);

        var command = new AdjustBonusV2Command(
            Guid.NewGuid(),
            100M,
            "新增優惠調帳",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        var description = JsonSerializer.Serialize(new
        {
            TransactionType = (int)TransactionType.AdjustBonus,
            Remark = command.Description
        });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.AdjustBonus.ToString()
                    && request.Amount == command.Amount
                    && request.Description == description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_Agent_Adjust_Exchange_V2()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new AdjustAgentCommandService(fakeWalletExchangeService);

        var command = new AdjustAgentV2Command(
            Guid.NewGuid(),
            100M,
            "新增代理調帳",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        var description = JsonSerializer.Serialize(new
        {
            TransactionType = (int)TransactionType.AdjustAgent,
            Remark = command.Description
        });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.AdjustAgent.ToString()
                    && request.Amount == command.Amount
                    && request.Description == description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_System_Adjust_Exchange_V2()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new AdjustSystemCommandService(fakeWalletExchangeService);

        var command = new AdjustSystemV2Command(
            Guid.NewGuid(),
            100M,
            0M,
            "新增系統調帳",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        var description = JsonSerializer.Serialize(new
        {
            TransactionType = (int)TransactionType.AdjustSystem,
            Remark = command.Description
        });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.AdjustSystem.ToString()
                    && request.Amount == command.Amount
                    && request.Description == description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_Transfer_Adjust_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new AdjustTransferCommandService(fakeWalletExchangeService);

        var command = new AdjustTransferCommand(
            Guid.NewGuid(),
            100M,
            "新增系統轉帳",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.AdjustTransfer.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }
}
