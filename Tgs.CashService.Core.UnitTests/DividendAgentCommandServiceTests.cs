using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

public class DividendAgentCommandServiceTests
{
    [Test]
    public async Task 代理分紅操作()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new DividendAgentCommandService(fakeWalletExchangeService);

        var command = new DividendAgentCommand(
            Guid.NewGuid(),
            1000M,
            null,
            "test_id");

        _ = await sut.GetAsync(command);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.WalletId == command.WalletId
                    && request.TransactionType == nameof(TransactionType.AgentDividend)
                    && request.Amount == command.Amount
                    && request.Description == string.Empty
                    && request.ExternalId == command.ExternalId));
    }
}
