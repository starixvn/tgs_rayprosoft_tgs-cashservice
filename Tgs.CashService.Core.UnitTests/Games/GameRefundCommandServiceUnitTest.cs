using NSubstitute;
using NUnit.Framework;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests.Games;

[TestFixture]
public class GameRefundCommandServiceUnitTest
{
    [Test]
    public async Task Can_Create_GameRefund_Is_Plus_100_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameRefundCommandService(fakeWalletExchangeService);

        var command = new GameRefundCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameRefund.ToString()
                           && request.Amount == command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameRefund_Is_Minus_100_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameRefundCommandService(fakeWalletExchangeService);

        var command = new GameRefundCommand(
            Guid.NewGuid(),
            -100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameRefund.ToString()
                           && request.Amount == command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }
}
