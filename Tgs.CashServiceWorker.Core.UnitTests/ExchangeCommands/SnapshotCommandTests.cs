using Microsoft.Extensions.DependencyInjection;
using Tgs.CashService;
using Tgs.CashServiceWorker.ExchangeCommands;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashServiceWorker.Core.UnitTests.ExchangeCommands;

public class SnapshotCommandTests
{
    [Test]
    public async Task SnapshotCommand_增加快照記錄()
    {
        var sut = new SnapshotCommand();

        var context = new StubWalletExchangeContext(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "test1",
            1000M,
            500M);

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var serviceProvider = new ServiceCollection()
            .AddSingleton(fakeWalletRepository)
            .AddSingleton(fakeSerialNumberGenerator)
            .BuildServiceProvider();

        await sut.ExecuteAsync(context, serviceProvider);
        var actual = await sut.GetExchangeResultAsync();

        Assert.NotNull(actual);
        Assert.AreEqual(context.Balance, actual.Balance);
        Assert.AreEqual(context.LockedBalance, actual.LockedBalance);
        _ = fakeWalletRepository.Received(1)
            .WriteTransactionRecordAsync(Arg.Is<TransactionRecord>(record => record.Amount == 0M
                && record.LockAmount == 0M
                && record.Type == TransactionType.Snapshot));
    }
}
