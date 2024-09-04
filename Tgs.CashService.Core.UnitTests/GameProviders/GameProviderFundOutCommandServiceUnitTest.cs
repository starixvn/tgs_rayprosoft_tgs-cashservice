using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderFundOutCommandServiceUnitTest
{
    [Test]
    public async Task GameProviderFundOutCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new GameProviderFundOutCommandService(fakeTgsRepository);

        var command = new GameProviderFundOutCommand(
            Guid.NewGuid(),
            1000M,
            null,
            null);

        var actual = await sut.GetAsync(command);

        _ = fakeTgsRepository.Received(1)
         .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
             record => record.WalletId == command.WalletId
                 && record.TransactionType == TransactionType.GameProviderFundOutCommit
                 && record.Amount == -command.Amount
                 && record.LockAmount == 0
                 && record.Description == string.Empty
                 && record.ExternalId == string.Empty));
    }
}
