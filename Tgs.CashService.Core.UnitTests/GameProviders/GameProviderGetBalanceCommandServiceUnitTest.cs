using NSubstitute.ReturnsExtensions;
using NUnit.Framework.Legacy;
using Tgs.CashService.ApplicationServices;
using Tgs.CashService.Core.Entities;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderGetBalanceCommandServiceUnitTest
{
    [Test]
    public async Task GameProviderGetBalanceCommandService_WhenInvalidPlayerCode_ReturnError()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var sut = new GameProviderGetBalanceCommandService(fakeTgsRepository);
        var command = new GameProviderGetBalanceCommand("player_code");
        fakeTgsRepository.GetWalletByPlayerCodeAsync(Arg.Is(command.PlayerCode)).ReturnsNull();
        var expected = new BalanceResult(Guid.Empty, 0, 0, ErrorCode.NotFoundWallet);

        var actual = await sut.GetAsync(command);

        ClassicAssert.AreEqual(ErrorCode.NotFoundWallet, actual.ErrorCode);
    }

    [Test]
    public async Task GameProviderGetBalanceCommandService_WhenValidPlayerCode_ReturnExpectedBalance()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var sut = new GameProviderGetBalanceCommandService(fakeTgsRepository);
        var command = new GameProviderGetBalanceCommand("player_code");
        var wallet = new Wallet { Balance = 1000, LockedBalance = 1 };
        fakeTgsRepository.GetWalletByPlayerCodeAsync(Arg.Is(command.PlayerCode)).Returns(wallet);
        var expected = new BalanceResult(Guid.Empty, wallet.Balance, wallet.LockedBalance, ErrorCode.Success);

        var actual = await sut.GetAsync(command);

        ClassicAssert.AreEqual(ErrorCode.Success, actual.ErrorCode);
        ClassicAssert.AreEqual(1000, actual.Balance);
        ClassicAssert.AreEqual(1, actual.LockedBalance);
    }
}
