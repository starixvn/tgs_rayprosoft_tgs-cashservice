using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests.Games;

[TestFixture]
public class GameBetCommandServiceUnitTest
{
    [Test]
    public async Task Can_Create_GameBet_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameBetCommandService(fakeWalletExchangeService);

        var command = new GameBetCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"));

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameBet.ToString()
                           && request.Amount == -command.Amount
                           && request.Description == string.Empty
                           && request.ExternalId == command.ExternalId));
    }
}
