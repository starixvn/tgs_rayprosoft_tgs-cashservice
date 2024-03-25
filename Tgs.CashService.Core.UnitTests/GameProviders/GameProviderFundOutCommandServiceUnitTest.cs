using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderFundOutCommandServiceUnitTest
{
    [Test]
    public async Task GameProviderFundOutCommandService_轉出錢包金額到遊戲()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderFundOutCommandService(fakeWalletExchangeService);

        var command = new GameProviderFundOutCommand(
            Guid.NewGuid(),
            1000M,
            null,
            null);

        _ = fakeWalletExchangeService.SendRequestAsync(Arg.Is<WalletExchangeRequest>(
            request => request.WalletId.ToGuid() == command.WalletId
                && request.TransactionType == TransactionType.GameProviderFundOutLock.ToString()
                && request.Amount == -command.Amount
                && request.LockAmount == command.Amount))
            .Returns(new ExchangeResult(Guid.NewGuid(), 0M, -1000M));

        _ = fakeWalletExchangeService.SendRequestAsync(Arg.Is<WalletExchangeRequest>(
            request => request.WalletId.ToGuid() == command.WalletId
                && request.TransactionType == TransactionType.GameProviderFundOutCommit.ToString()
                && request.Amount == 0M
                && request.LockAmount == -command.Amount))
            .Returns(new ExchangeResult(Guid.NewGuid(), 0M, 0M));

        var actual = await sut.GetAsync(command);

        Assert.That(actual.Balance, Is.EqualTo(0M));
        Assert.That(actual.LockedBalance, Is.EqualTo(0M));
    }

    [Test]
    public void GameProviderFundOutCommandService_轉出錢包金額到遊戲_如果轉出後餘額變負數_會恢復金額並且發生例外()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderFundOutCommandService(fakeWalletExchangeService);

        var command = new GameProviderFundOutCommand(
            Guid.NewGuid(),
            1000M,
            null,
            null);

        _ = fakeWalletExchangeService.SendRequestAsync(Arg.Is<WalletExchangeRequest>(
            request => request.WalletId.ToGuid() == command.WalletId
                && request.TransactionType == TransactionType.GameProviderFundOutLock.ToString()
                && request.Amount == -command.Amount
                && request.LockAmount == command.Amount))
            .Returns(new ExchangeResult(Guid.NewGuid(), -10M, -1000M));

        _ = Assert.ThrowsAsync<Exception>(() => sut.GetAsync(command).AsTask());

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId.ToGuid() == command.WalletId
                && request.TransactionType == TransactionType.GameProviderFundOutRollback.ToString()
                && request.Amount == command.Amount
                && request.LockAmount == -command.Amount));
        _ = fakeWalletExchangeService.DidNotReceive()
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                req => req.TransactionType == TransactionType.GameProviderFundOutCommit.ToString()));
    }
}
