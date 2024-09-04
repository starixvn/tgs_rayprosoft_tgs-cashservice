using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class PaymentTests
{
    [Test]
    [Obsolete("已淘汰")]
    public async Task DepositCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new DepositCommandService(fakeTgsRepository);

        var command = new DepositCommand(
            Guid.NewGuid(),
            100M,
            "測試存款",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
             .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                 record => record.WalletId == command.WalletId
                 && record.TransactionType == TransactionType.DepositOld
                 && record.Amount == command.Amount
                 && record.LockAmount == 0
                 && record.Description == command.Description
                 && record.ExternalId == command.ExternalId
                 && record.UserId == command.CreatedByUserId));
    }

    [Test]
    public async Task DepositedCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new DepositedCommandService(fakeTgsRepository);

        var command = new DepositedCommand(
            Guid.NewGuid(),
            100M,
            "測試存款",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.Deposit
                  && record.Amount == command.Amount
                  && record.LockAmount == 0
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId
                  && record.UserId == command.CreatedByUserId));
    }

    [Test]
    [Obsolete("已淘汰")]
    public async Task WithdrawalCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new WithdrawalCommandService(fakeTgsRepository);

        var command = new WithdrawalCommand(
            Guid.NewGuid(),
            100,
            "測試提款",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.Withdrawal
                  && record.Amount == -command.Amount
                  && record.LockAmount == 0
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task WithdrawLockCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new WithdrawLockCommandService(fakeTgsRepository);

        var command = new WithdrawLockCommand(
            Guid.NewGuid(),
            100,
            "測試提款申請",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.WithdrawLock
                  && record.Amount == -command.Amount
                  && record.LockAmount == command.Amount
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId));
    }

    [Test]
    [Obsolete("已淘汰")]
    public async Task CancelWithdrawalCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new CancelWithdrawalCommandService(fakeTgsRepository);

        var command = new CancelWithdrawalCommand(
            Guid.NewGuid(),
            100M,
            "測試取消提款單",
            Guid.NewGuid().ToString("N"),
            Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
               .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                   record => record.WalletId == command.WalletId
                   && record.TransactionType == TransactionType.CancelWithdrawal
                   && record.Amount == command.Amount
                   && record.LockAmount == 0
                   && record.Description == command.Description
                   && record.ExternalId == command.ExternalId
                   && record.UserId == command.CreatedByUserId));
    }

    [Test]
    public async Task CommitWithdrawCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new CommitWithdrawCommandService(fakeTgsRepository);

        var command = new CommitWithdrawCommand(
            Guid.NewGuid(),
            100M,
            80M,
            Guid.NewGuid().ToString("N"),
            "測試通過提款單",
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
              .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                  record => record.WalletId == command.WalletId
                  && record.TransactionType == TransactionType.WithdrawCommit
                  && record.Amount == 0
                  && record.LockAmount == -command.Amount
                  && record.Description == command.Description
                  && record.ExternalId == command.ExternalId
                  && record.UserId == command.CreatedByUserId));
    }

    [Test]
    public async Task RollbackWithdrawCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new RollbackWithdrawCommandService(fakeTgsRepository);

        var command = new RollbackWithdrawCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試拒絕提款單",
            Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
               .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                   record => record.WalletId == command.WalletId
                   && record.TransactionType == TransactionType.WithdrawRollback
                   && record.Amount == command.Amount
                   && record.LockAmount == -command.Amount
                   && record.Description == command.Description
                   && record.ExternalId == command.ExternalId));
    }
}
