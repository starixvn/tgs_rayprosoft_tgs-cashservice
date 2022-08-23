using NSubstitute;
using NUnit.Framework;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderDepositCommandServiceUnitTest
{
    [Test]
    public async Task Can_Create_GameProviderDeposit_Exchange()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new GameProviderDepositCommandService(fakeWalletExchangeService);

        var command = new GameProviderDepositCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試遊戲上分");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                           && request.TransactionType == TransactionType.GameProviderDeposit.ToString()
                           && request.Amount == -command.Amount
                           && request.Description == command.Description
                           && request.ExternalId == command.ExternalId));
    }
}
