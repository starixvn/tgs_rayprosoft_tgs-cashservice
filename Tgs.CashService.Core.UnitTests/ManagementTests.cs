using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class ManagementTests
{
    [Test]
    public async Task BonusCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new BonusCommandService(fakeTgsRepository);

        var command = new BonusCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "6B95C992-0B62-4EED-943F-64D6C7A21FF5");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
            .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                record => record.WalletId == command.WalletId
                && record.TransactionType == TransactionType.Bonus
                && record.Amount == command.Amount
                && record.LockAmount == 0
                && record.Description == command.Description
                && record.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task RebateCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new RebateCommandService(fakeTgsRepository);

        var command = new RebateCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "6B95C992-0B62-4EED-943F-64D6C7A21FF5");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
            .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                record => record.WalletId == command.WalletId
                && record.TransactionType == TransactionType.Rebate
                && record.Amount == command.Amount.Truncate(2)
                && record.LockAmount == 0
                && record.Description == command.Description
                && record.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task OtherCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new OtherCommandService(fakeTgsRepository);

        var command = new OtherCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "6B95C992-0B62-4EED-943F-64D6C7A21FF5",
            Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
            .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                record => record.WalletId == command.WalletId
                && record.TransactionType == TransactionType.Other
                && record.Amount == command.Amount
                && record.LockAmount == 0
                && record.Description == command.Description
                && record.ExternalId == command.ExternalId
                && record.UserId == command.CreatedByUserId));
    }

    [Test]
    public async Task UnsettledBetManualCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new UnsettledBetManualCommandService(fakeTgsRepository);

        var command = new UnsettledBetManualCommand(
            Guid.NewGuid(),
            100,
            "Open Round Page",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
             .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                 record => record.WalletId == command.WalletId
                 && record.TransactionType == TransactionType.UnsettledBetManual
                 && record.Amount == command.Amount
                 && record.LockAmount == 0
                 && record.Description == command.Description
                 && record.ExternalId == command.ExternalId
                 && record.UserId == command.CreatedByUserId));
    }

    [Test]
    public async Task AdjustResettledBetManualCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new AdjustResettledBetManualCommandService(fakeTgsRepository);

        var command = new AdjustResettledBetManualCommand(
            Guid.NewGuid(),
            100,
            "Closed Round Page",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.AdjustResettledBetManual
                  && record.Amount == command.Amount
                  && record.LockAmount == 0
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId
                  && record.UserId == command.CreatedByUserId));
    }
}
