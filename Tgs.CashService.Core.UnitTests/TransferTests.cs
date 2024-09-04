using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests;

public class TransferTests
{
    [Test]
    public async Task Can_Create_TransferLock_Exchange()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new TransferLockCommandService(fakeTgsRepository);

        var command = new TransferLockCommand(
            Guid.NewGuid(),
            100M,
            "測試開始轉帳交易",
            Guid.NewGuid(),
            Guid.NewGuid());

        _ = await sut.GetAsync(command);

        _ = fakeTgsRepository.Received(1)
             .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                 record => record.WalletId == command.WalletId
                 && record.TransactionType == TransactionType.Transfer
                 && record.Amount == -command.Amount
                 && record.LockAmount == command.Amount
                 && record.Description == command.Description
                 && record.ExternalId == command.ExternalId.ToString()
                 && record.UserId == command.Executor));
    }

    [Test]
    public async Task Can_Create_TransferCommit_Exchange()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new TransferCommitCommandService(fakeTgsRepository);

        var command = new TransferCommitCommand(
            Guid.NewGuid(),
            100M,
            "測試完成轉帳交易",
            Guid.NewGuid(),
            Guid.NewGuid());

        _ = await sut.GetAsync(command);
        
        _ = fakeTgsRepository.Received(1)
             .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                 record => record.WalletId == command.WalletId
                 && record.TransactionType == TransactionType.TransferCommit
                 && record.Amount == 0
                 && record.LockAmount == -command.Amount
                 && record.Description == command.Description
                 && record.ExternalId == command.ExternalId.ToString()
                 && record.UserId == command.Executor));
    }

    [Test]
    public async Task Can_Create_TransferRollback_Exchange()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new TransferRollbackCommandService(fakeTgsRepository);

        var command = new TransferRollbackCommand(
            Guid.NewGuid(),
            100M,
            "測試回滾轉帳交易",
            Guid.NewGuid(),
            Guid.NewGuid());

        _ = await sut.GetAsync(command);

        _ = fakeTgsRepository.Received(1)
             .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                 record => record.WalletId == command.WalletId
                 && record.TransactionType == TransactionType.TransferRollback
                 && record.Amount == command.Amount
                 && record.LockAmount == -command.Amount
                 && record.Description == command.Description
                 && record.ExternalId == command.ExternalId.ToString()
                 && record.UserId == command.Executor));
    }

    [Test]
    public async Task Can_Create_ReceiveLock_Exchange()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new ReceiveLockCommandService(fakeTgsRepository);

        var command = new ReceiveLockCommand(
            Guid.NewGuid(),
            100M,
            "測試開始轉帳交易",
            Guid.NewGuid(),
            Guid.NewGuid());

        _ = await sut.GetAsync(command);

        _ = fakeTgsRepository.Received(1)
            .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                record => record.WalletId == command.WalletId
                && record.TransactionType == TransactionType.Receive
                && record.Amount == 0
                && record.LockAmount == command.Amount
                && record.Description == command.Description
                && record.ExternalId == command.ExternalId.ToString()
                && record.UserId == command.Executor));
    }

    [Test]
    public async Task Can_Create_ReceiveCommit_Exchange()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new ReceiveCommitCommandService(fakeTgsRepository);

        var command = new ReceiveCommitCommand(
            Guid.NewGuid(),
            100M,
            "測試完成轉帳交易",
            Guid.NewGuid(),
            Guid.NewGuid());

        _ = await sut.GetAsync(command);

        _ = fakeTgsRepository.Received(1)
            .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                record => record.WalletId == command.WalletId
                && record.TransactionType == TransactionType.ReceiveCommit
                && record.Amount == command.Amount
                && record.LockAmount == -command.Amount
                && record.Description == command.Description
                && record.ExternalId == command.ExternalId.ToString()
                && record.UserId == command.Executor));
    }

    [Test]
    public async Task Can_Create_ReceiveRollback_Exchange()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new ReceiveRollbackCommandService(fakeTgsRepository);

        var command = new ReceiveRollbackCommand(
            Guid.NewGuid(),
            100M,
            "測試回滾轉帳交易",
            Guid.NewGuid(),
            Guid.NewGuid());

        _ = await sut.GetAsync(command);

        _ = fakeTgsRepository.Received(1)
            .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
                record => record.WalletId == command.WalletId
                && record.TransactionType == TransactionType.ReceiveRollback
                && record.Amount == 0
                && record.LockAmount == -command.Amount
                && record.Description == command.Description
                && record.ExternalId == command.ExternalId.ToString()
                && record.UserId == command.Executor));
    }
}
