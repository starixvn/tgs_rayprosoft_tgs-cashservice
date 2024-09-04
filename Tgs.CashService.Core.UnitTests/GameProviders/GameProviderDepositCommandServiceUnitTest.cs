using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderDepositCommandServiceUnitTest
{
    [Test]
    public async Task GameProviderDepositCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new GameProviderDepositCommandService(fakeTgsRepository);

        var command = new GameProviderDepositCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試遊戲上分");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
           .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == command.WalletId
               && record.TransactionType == TransactionType.GameProviderDeposit
               && record.Amount == -command.Amount
                && record.LockAmount == 0
               && record.Description == command.Description
               && record.ExternalId == command.ExternalId));
    }
}
