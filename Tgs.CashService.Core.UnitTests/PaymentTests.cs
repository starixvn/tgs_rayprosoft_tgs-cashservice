using NSubstitute;
using NUnit.Framework;
using Tgs.Cash.Protos;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class PaymentTests
{
    [Test]
    public async Task Can_Create_Deposit_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new DepositCommandService(fakeWalletRepository, fakeWalletExchangeService);

        var command = new DepositCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試存款",
            Guid.NewGuid());

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Deposit.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }

    [Test]
    public void Create_Deposit_Exchange_With_Invalid_WalletId_Will_Throw_Exception()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new DepositCommandService(fakeWalletRepository, fakeWalletExchangeService);

        var command = new DepositCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試存款",
            null);

        _ = Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.GetAsync(command));

        _ = fakeWalletExchangeService.DidNotReceive()
            .SendRequestAsync(Arg.Any<WalletExchangeRequest>());
    }

    [Test]
    public async Task Can_Create_Withdrawal_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new WithdrawalCommandService(fakeWalletRepository, fakeWalletExchangeService);

        var command = new WithdrawalCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試反水");

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.Withdrawal.ToString()
                    && request.Amount == -command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_CancelWithdrawal_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new CancelWithdrawalCommandService(fakeWalletRepository, fakeWalletExchangeService);

        var command = new CancelWithdrawalCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試取消提款單",
            Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                BrandId = Guid.NewGuid(),
                Id = command.WalletId,
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == TransactionType.CancelWithdrawal.ToString()
                    && request.Amount == command.Amount
                    && request.Description == command.Description
                    && request.ExternalId == command.ExternalId
                    && request.CreatedUser == command.CreatedByUserId));
    }
}
