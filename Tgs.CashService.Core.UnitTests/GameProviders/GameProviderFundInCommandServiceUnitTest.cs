using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderFundInCommandServiceUnitTest
{
    [Test]
    public async Task GameProviderFundInCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new GameProviderFundInCommandService(fakeTgsRepository);

        var command = new GameProviderFundInCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試轉入主錢包");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
          .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
              record => record.WalletId == command.WalletId
              && record.TransactionType == TransactionType.GameProviderFundIn
              && record.Amount == command.Amount
               && record.LockAmount == 0
              && record.Description == command.Description
              && record.ExternalId == command.ExternalId));
    }
}
