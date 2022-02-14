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
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new DepositCommandService(fakeWalletExchangeService);

        var command = new DepositCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試存款",
            Guid.NewGuid());

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
    public async Task Can_Create_Withdrawal_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new WithdrawalCommandService(fakeWalletExchangeService);

        var command = new WithdrawalCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試反水");

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
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new CancelWithdrawalCommandService(fakeWalletExchangeService);

        var command = new CancelWithdrawalCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試取消提款單",
            Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

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
