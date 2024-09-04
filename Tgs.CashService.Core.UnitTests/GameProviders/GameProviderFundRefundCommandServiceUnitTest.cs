using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderFundRefundCommandServiceUnitTest
{
    [Test]
    public async Task GameProviderFundRefundCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new GameProviderFundRefundCommandService(fakeTgsRepository);

        var command = new GameProviderFundRefundCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試資金退款");

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeTgsRepository.Received(1)
        .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
            record => record.WalletId == command.WalletId
                && record.TransactionType == TransactionType.GameProviderFundRefund
                && record.Amount == command.Amount
                && record.LockAmount == 0
                && record.Description == command.Description
                && record.ExternalId == command.ExternalId));
    }
}
