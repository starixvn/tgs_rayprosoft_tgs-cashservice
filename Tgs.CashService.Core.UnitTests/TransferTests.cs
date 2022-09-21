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
            -100M,
            "測試開始轉帳交易",
            Guid.NewGuid(),
            Guid.NewGuid());

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Transfer.ToString()
                    && request.Amount == command.Amount
                    && request.LockAmount == command.LockAmount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId.ToString()
                    && request.CreatedUser == command.Executor));
    }
}
