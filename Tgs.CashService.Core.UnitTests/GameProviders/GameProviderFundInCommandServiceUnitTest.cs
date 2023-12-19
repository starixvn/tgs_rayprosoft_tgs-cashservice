using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderFundInCommandServiceUnitTest
{
    [Test]
    public async Task Can_Create_GameProviderFundIn_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderFundInCommandService(fakeWalletExchangeService);

        var command = new GameProviderFundInCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試轉入主錢包");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameProviderFundIn.ToString()
                           && request.Amount == command.Amount
                           && request.Description == command.Description
                           && request.ExternalId == command.ExternalId));
    }
}
