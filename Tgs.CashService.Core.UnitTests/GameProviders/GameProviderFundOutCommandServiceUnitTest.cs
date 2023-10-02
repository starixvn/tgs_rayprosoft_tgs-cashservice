using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderFundOutCommandServiceUnitTest
{
    [Test]
    public async Task Can_Create_GameProviderFundOut_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderFundOutCommandService(fakeWalletExchangeService);

        var command = new GameProviderFundOutCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試轉出主錢包");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameProviderFundOut.ToString()
                           && request.Amount == -command.Amount
                           && request.Description == command.Description
                           && request.ExternalId == command.ExternalId));
    }
}
