using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework.Legacy;
using Tgs.CashService;
using Tgs.CashServiceWorker.ExchangeCommands;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashServiceWorker.Core.UnitTests.ExchangeCommands;

public class DeductBalanceCommandTests
{
    [Test]
    public async Task DeductBalanceCommand_WhenBalanceGreaterThanAmount_UpdateBalance()
    {
        var request = new ExchangeRequest(
            Guid.NewGuid(),
            TransactionType.GameProviderDeductBalance,
            10M,
            0M,
            null,
            null,
            null);

        var sut = new DeductBalanceCommand(request, null);

        var context = new StubWalletExchangeContext(
            request.WalletId,
            Guid.NewGuid(),
            "test1",
            100M,
            0M);

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var serviceProvider = new ServiceCollection()
            .AddSingleton(fakeWalletRepository)
            .AddSingleton(fakeSerialNumberGenerator)
            .BuildServiceProvider();

        await sut.ExecuteAsync(context, serviceProvider);
        var actual = await sut.GetExchangeResultAsync();

        ClassicAssert.NotNull(actual);
        ClassicAssert.AreEqual(90M, actual.Balance);
        _ = fakeWalletRepository.Received(1)
            .WriteTransactionRecordAsync(Arg.Is<TransactionRecord>(record => record.Amount == request.Amount
                && record.LockAmount == request.LockAmount
                && record.Type == request.TransactionType
                && record.Balance == 90M
                && record.LockedBalance == 0M));
    }

    [Test]
    public void DeductBalanceCommand_WhenBalanceLessThanAmount_ThrowException()
    {
        var request = new ExchangeRequest(
            Guid.NewGuid(),
            TransactionType.WithdrawCommit,
            10M,
            0M,
            null,
            null,
            null);

        var sut = new DeductBalanceCommand(request, null);

        var context = new StubWalletExchangeContext(
            request.WalletId,
            Guid.NewGuid(),
            "test1",
            9M,
            0M);

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var serviceProvider = new ServiceCollection()
            .AddSingleton(fakeWalletRepository)
            .AddSingleton(fakeSerialNumberGenerator)
            .BuildServiceProvider();

        _ = Assert.ThrowsAsync<WalletExchangeFailException>(async () => await sut.ExecuteAsync(context, serviceProvider));

        ClassicAssert.AreEqual(9M, context.Balance);
        ClassicAssert.AreEqual(0M, context.LockedBalance);
        _ = fakeWalletRepository.Received(0)
            .WriteTransactionRecordAsync(Arg.Any<TransactionRecord>());
    }
}
