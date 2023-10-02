using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests.Games;

[TestFixture]
public class GameRevisionCommandServiceUnitTest
{
    [Test]
    public async Task Can_Create_GameRevision_Is_Plus_100_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameRevisionCommandService(fakeWalletExchangeService);

        var command = new GameRevisionCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameRevision.ToString()
                           && request.Amount == command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameRevision_Is_Minus_100_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameRevisionCommandService(fakeWalletExchangeService);

        var command = new GameRevisionCommand(
            Guid.NewGuid(),
            -100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameRevision.ToString()
                           && request.Amount == command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }
}
