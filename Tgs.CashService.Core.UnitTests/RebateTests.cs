using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class RebateTests
{
    [Test]
    public async Task RebateCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new RebateCommandService(fakeTgsRepository);

        var command = new RebateCommand(
            Guid.NewGuid(),
            100M,
            "新增玩家返水",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.Rebate
                  && record.Amount == command.Amount
                  && record.LockAmount == 0
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task DividendAllAgentRebateCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new DividendAllAgentRebateCommandService(fakeTgsRepository);

        var command = new DividendAllAgentRebateCommand(
            Guid.NewGuid(),
            100M,
            "新增玩家全民代理返水",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.DividendAllAgentRebate
                  && record.Amount == command.Amount
                  && record.LockAmount == 0
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task RebateCommandService_TruncateAmount_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new RebateCommandService(fakeTgsRepository);

        var command = new RebateCommand(
            Guid.NewGuid(),
            100.123456789M,
            "新增玩家返水",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.Rebate
                  && record.Amount == 100.12M
                  && record.LockAmount == 0
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task DividendAllAgentRebateCommandService_TruncateAmount_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new DividendAllAgentRebateCommandService(fakeTgsRepository);

        var command = new DividendAllAgentRebateCommand(
            Guid.NewGuid(),
            100.123456789M,
            "新增玩家全民代理返水",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.DividendAllAgentRebate
                  && record.Amount == 100.12M
                  && record.LockAmount == 0
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId));
    }
}
