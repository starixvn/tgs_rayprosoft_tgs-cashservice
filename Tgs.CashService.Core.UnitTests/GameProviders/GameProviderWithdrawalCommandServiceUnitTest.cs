using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderWithdrawalCommandServiceUnitTest
{
    [Test]
    public async Task GameProviderWithdrawalCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new GameProviderWithdrawalCommandService(fakeTgsRepository);

        var command = new GameProviderWithdrawalCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試遊戲下分");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
        .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
            record => record.WalletId == command.WalletId
                && record.TransactionType == TransactionType.GameProviderWithdrawal
                && record.Amount == command.Amount
                && record.LockAmount == 0
                && record.Description == command.Description
                && record.ExternalId == command.ExternalId));
    }
}
