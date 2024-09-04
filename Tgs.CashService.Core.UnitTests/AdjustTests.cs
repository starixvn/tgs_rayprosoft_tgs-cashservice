using System.Text.Json;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class AdjustTests
{
    [Test]
    public async Task AdjustCompanyCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        
        var sut = new AdjustCompanyCommandService(fakeTgsRepository);

        var command = new AdjustCompanyV2Command(
            Guid.NewGuid(),
            100M,
            "AdjustCompanyV2Command",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        var description = JsonSerializer.Serialize(new
        {
            TransactionType = (int)TransactionType.AdjustCompany,
            Remark = command.Description
        });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
            .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                record => record.WalletId == command.WalletId
                && record.TransactionType == TransactionType.AdjustCompany
                && record.Amount == command.Amount
                && record.LockAmount == 0
                && record.Description == description
                && record.ExternalId == command.ExternalId
                && record.UserId == command.CreatedByUserId));
    }

    [Test]
    public async Task AdjustBonusCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustBonusCommandService(fakeTgsRepository);

        var command = new AdjustBonusV2Command(
            Guid.NewGuid(),
            100M,
            "AdjustBonusCommandService",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        var description = JsonSerializer.Serialize(new
        {
            TransactionType = (int)TransactionType.AdjustBonus,
            Remark = command.Description
        });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
           .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == command.WalletId
               && record.TransactionType == TransactionType.AdjustBonus
               && record.Amount == command.Amount
                && record.LockAmount == 0
               && record.Description == description
               && record.ExternalId == command.ExternalId
               && record.UserId == command.CreatedByUserId));
    }

    [Test]
    public async Task AdjustAgentCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustAgentCommandService(fakeTgsRepository);

        var command = new AdjustAgentV2Command(
            Guid.NewGuid(),
            100M,
            "AdjustAgentV2Command",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        var description = JsonSerializer.Serialize(new
        {
            TransactionType = (int)TransactionType.AdjustAgent,
            Remark = command.Description
        });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
           .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == command.WalletId
               && record.TransactionType == TransactionType.AdjustAgent
               && record.Amount == command.Amount
                && record.LockAmount == 0
               && record.Description == description
               && record.ExternalId == command.ExternalId
               && record.UserId == command.CreatedByUserId));
    }

    [Test]
    public async Task AdjustSystemCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustSystemCommandService(fakeTgsRepository);

        var command = new AdjustSystemV2Command(
            Guid.NewGuid(),
            100M,
            0M,
            "AdjustSystemV2Command",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        var description = JsonSerializer.Serialize(new
        {
            TransactionType = (int)TransactionType.AdjustSystem,
            Remark = command.Description
        });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
           .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == command.WalletId
               && record.TransactionType == TransactionType.AdjustSystem
               && record.Amount == command.Amount
                && record.LockAmount == 0
               && record.Description == description
               && record.ExternalId == command.ExternalId
               && record.UserId == command.CreatedByUserId));
    }

    [Test]
    public async Task AdjustTransferCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustTransferCommandService(fakeTgsRepository);

        var command = new AdjustTransferCommand(
            Guid.NewGuid(),
            100M,
            "AdjustTransferCommandService",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
           .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == command.WalletId
               && record.TransactionType == TransactionType.AdjustTransfer
               && record.Amount == command.Amount
                && record.LockAmount == 0
               && record.Description == command.Description
               && record.ExternalId == command.ExternalId
               && record.UserId == command.CreatedByUserId));
    }
}
