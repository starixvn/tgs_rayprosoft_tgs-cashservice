using Microsoft.Extensions.DependencyInjection;
using Tgs.CashService;
using Tgs.CashServiceWorker.ExchangeCommands;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashServiceWorker.Core.UnitTests.ExchangeCommands;

public class TradeCommandTests
{
    [Test]
    public async Task TradeCommand_執行錢包交易()
    {
        var request = new ExchangeRequest(
            Guid.NewGuid(),
            TransactionType.Deposit,
            1000M,
            0M,
            null,
            null,
            null);

        var sut = new TradeCommand(request);

        var context = new StubWalletExchangeContext(
            request.WalletId,
            Guid.NewGuid(),
            "test1",
            0M,
            0M);

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var serviceProvider = new ServiceCollection()
            .AddSingleton(fakeWalletRepository)
            .AddSingleton(fakeSerialNumberGenerator)
            .BuildServiceProvider();

        await sut.ExecuteAsync(context, serviceProvider);
        var actual = await sut.GetExchangeResultAsync();

        Assert.NotNull(actual);
        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(0M, actual.LockedBalance);
        _ = fakeWalletRepository.Received(1)
            .WriteTransactionRecordAsync(Arg.Is<TransactionRecord>(record => record.Amount == request.Amount
                && record.LockAmount == request.LockAmount
                && record.Type == request.TransactionType
                && record.Balance == 1000M
                && record.LockedBalance == 0M));
    }
}
