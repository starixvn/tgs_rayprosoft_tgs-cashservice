using NSubstitute;
using NUnit.Framework;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests.Games
{
    [TestFixture]
    public class GameSettleFreeCommandServiceUnitTest
    {
        [Test]
        public async Task Can_Create_GameSettleFree_Exchange()
        {
            var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

            var sut = new GameSettleFreeCommandService(fakeWalletExchangeService);

            var command = new GameSettleFreeCommand(
                Guid.NewGuid(),
                100M,
                Guid.NewGuid().ToString("N"));

            _ = await sut.GetAsync(command).ConfigureAwait(false);

            _ = fakeWalletExchangeService.Received(1)
                .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                    request => request.WalletId == command.WalletId
                               && request.TransactionType == TransactionType.GameSettleFree.ToString()
                               && request.Amount == command.Amount
                               && request.Description == string.Empty
                               && request.ExternalId == command.ExternalId));
        }
    }
}
