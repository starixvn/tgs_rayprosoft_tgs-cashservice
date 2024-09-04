using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Models;

namespace Tgs.CashService.Core.UnitTests;

public class DividendAgentCommandServiceTests
{
    [Test]
    public async Task DividendAgentCommandService_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new DividendAgentCommandService(fakeTgsRepository);

        var command = new DividendAgentCommand(
            Guid.NewGuid(),
            1000M,
            null,
            "test_id");

        _ = await sut.GetAsync(command);

        _ = fakeTgsRepository.Received(1)
           .ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == command.WalletId
               && record.TransactionType == TransactionType.AgentDividend
               && record.Amount == command.Amount
                && record.LockAmount == 0
               && record.Description == string.Empty
               && record.ExternalId == command.ExternalId));
    }
}
