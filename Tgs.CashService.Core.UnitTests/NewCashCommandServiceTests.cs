using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

public class NewCashCommandServiceTests
{
    [Test]
    public async Task 建立一個新的錢包帳號()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

        var sut = new NewCashCommandService(fakeTgsRepository, fakeWalletExchangeService);

        var command = new NewCashCommand(Guid.NewGuid(), "test1", CashType.Player, "CNY");

        var actual = await sut.GetAsync(command);

        _ = fakeTgsRepository.Received(1)
            .CreateWalletAsync(
                Arg.Is(actual),
                Arg.Is(command.BrandId),
                Arg.Is(command.UserId),
                Arg.Is(command.Type),
                Arg.Is(command.CurrencyCode));
        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is<WalletExchangeRequest>(
                request => request.TransactionType == nameof(TransactionType.CreateWallet)
                    && request.Amount.ToDecimal() == 0M
                    && request.LockAmount.ToDecimal() == 0M));
    }
}
