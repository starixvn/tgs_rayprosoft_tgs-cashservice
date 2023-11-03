using NSubstitute;
using Tgs.CashService.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

public class NewCashCommandServiceTests
{
    [Test]
    public async Task 建立一個新的錢包帳號()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new NewCashCommandService(fakeTgsRepository);

        var command = new NewCashCommand("test1", "CNY");

        var actual = await sut.GetAsync(command);

        _ = fakeTgsRepository.Received(1)
            .CreateWalletAsync(
                Arg.Is(actual),
                Arg.Is(command.UserId),
                Arg.Is(command.CurrencyCode));
    }
}
