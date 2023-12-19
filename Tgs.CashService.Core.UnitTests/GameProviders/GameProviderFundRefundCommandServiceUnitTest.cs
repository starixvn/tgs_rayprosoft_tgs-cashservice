using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderFundRefundCommandServiceUnitTest
{
    [Test]
    public async Task Can_Create_GameProviderFundRefund_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderFundRefundCommandService(fakeWalletExchangeService);

        var command = new GameProviderFundRefundCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試資金退款");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameProviderFundRefund.ToString()
                           && request.Amount == command.Amount
                           && request.Description == command.Description
                           && request.ExternalId == command.ExternalId));
    }
}
