using NSubstitute;
using NUnit.Framework;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class PaymentTests
{
    [Test]
    public async Task Can_Create_Deposit_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new DepositCommandService(fakeWalletExchangeService);

        var command = new DepositCommand(
            Guid.NewGuid(),
            100M,
            "測試存款",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Deposit.ToString()
                    && request.Amount == command.Amount
                    && request.LockAmount == 0M
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_Deposited_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new DepositedCommandService(fakeWalletExchangeService);

        var command = new DepositedCommand(
            Guid.NewGuid(),
            100M,
            "測試存款",
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Deposited.ToString()
                    && request.Amount == command.Amount
                    && request.LockAmount == 0M
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_Withdrawal_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new WithdrawalCommandService(fakeWalletExchangeService);

        var command = new WithdrawalCommand(
            Guid.NewGuid(),
            100,
            "測試提款",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Withdrawal.ToString()
                    && request.Amount == -command.Amount
                    && request.LockAmount == 0M
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_WithdrawLock_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new WithdrawLockCommandService(fakeWalletExchangeService);

        var command = new WithdrawLockCommand(
            Guid.NewGuid(),
            100,
            "測試提款申請",
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.WithdrawLock.ToString()
                    && request.Amount == -command.Amount
                    && request.LockAmount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_CancelWithdrawal_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new CancelWithdrawalCommandService(fakeWalletExchangeService);

        var command = new CancelWithdrawalCommand(
            Guid.NewGuid(),
            100M,
            "測試取消提款單",
            Guid.NewGuid().ToString("N"),
            Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.CancelWithdrawal.ToString()
                    && request.Amount == command.Amount
                    && request.LockAmount == 0M
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_WithdrawCommit_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new CommitWithdrawCommandService(fakeWalletExchangeService);

        var command = new CommitWithdrawCommand(
            Guid.NewGuid(),
            100M,
            80M,
            Guid.NewGuid().ToString("N"),
            "測試通過提款單",
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.WithdrawCommit.ToString()
                    && request.Amount == 0M
                    && request.LockAmount == -command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_WithdrawRollback_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new RollbackWithdrawCommandService(fakeWalletExchangeService);

        var command = new RollbackWithdrawCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試拒絕提款單",
            Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.WithdrawRollback.ToString()
                    && request.Amount == command.Amount
                    && request.LockAmount == -command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }
}
