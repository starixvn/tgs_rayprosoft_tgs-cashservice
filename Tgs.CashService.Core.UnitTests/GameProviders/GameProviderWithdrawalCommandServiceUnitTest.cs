using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderWithdrawalCommandServiceUnitTest
{
    [Test]
    public async Task Can_Create_GameProviderWithdrawal_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderWithdrawalCommandService(fakeWalletExchangeService);

        var command = new GameProviderWithdrawalCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試遊戲下分");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameProviderWithdrawal.ToString()
                           && request.Amount == command.Amount
                           && request.Description == command.Description
                           && request.ExternalId == command.ExternalId));
    }
}
