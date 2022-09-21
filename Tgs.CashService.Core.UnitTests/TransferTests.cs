using NSubstitute;
using NUnit.Framework;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

public class TransferTests
{
    [Test]
    public async Task Can_Create_TranferLock_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new TransferLockCommandService(fakeWalletExchangeService);

        var command = new TransferLockCommand(
            Guid.NewGuid(),
            100M,
            "測試開始轉帳交易",
            Guid.NewGuid(),
            Guid.NewGuid());

        _ = await sut.GetAsync(command);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Transfer.ToString()
                    && request.Amount == -command.Amount
                    && request.LockAmount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId.ToString()
                    && request.CreatedUser == command.Executor));
    }

    [Test]
    public async Task Can_Create_TranferCommit_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new TransferCommitCommandService(fakeWalletExchangeService);

        var command = new TransferCommitCommand(
            Guid.NewGuid(),
            100M,
            "測試完成轉帳交易",
            Guid.NewGuid(),
            Guid.NewGuid());

        _ = await sut.GetAsync(command);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.TransferCommit.ToString()
                    && request.Amount == 0M
                    && request.LockAmount == -command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId.ToString()
                    && request.CreatedUser == command.Executor));
    }
}
