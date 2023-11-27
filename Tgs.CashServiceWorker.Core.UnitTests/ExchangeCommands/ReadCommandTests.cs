using NUnit.Framework.Legacy;
using Tgs.CashServiceWorker.ExchangeCommands;

namespace Tgs.CashServiceWorker.Core.UnitTests.ExchangeCommands;

public class ReadCommandTests
{
    [Test]
    public async Task ReadCommand_讀取錢包餘額()
    {
        var sut = new ReadCommand();

        var context = new StubWalletExchangeContext(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "test1",
            1000M,
            500M);
        var fakeServiceProvider = Substitute.For<IServiceProvider>();

        await sut.ExecuteAsync(context, fakeServiceProvider);
        var actual = await sut.GetBalanceResultAsync();

        ClassicAssert.IsNotNull(actual);
        ClassicAssert.AreEqual(context.Balance, actual.Balance);
        ClassicAssert.AreEqual(context.LockedBalance, actual.LockedBalance);
    }
}
