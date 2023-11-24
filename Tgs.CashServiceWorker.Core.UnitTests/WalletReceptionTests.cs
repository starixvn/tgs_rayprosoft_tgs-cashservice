using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService;
using Tgs.CashServiceWorker.ExchangeCommands;
using Tgs.SerialNumberGenerator;

namespace Tgs.CashServiceWorker.Core.UnitTests;

public class WalletReceptionTests
{
    [Test]
    public async Task WalletReception_初始化後會取得錢包最新餘額()
    {
        var services = new ServiceCollection();

        var fakeTgsRepository = Substitute.For<IWalletRepository>();

        var sut = new WalletReception(
            Guid.NewGuid(),
            services
                .AddSingleton(fakeTgsRepository)
                .BuildServiceProvider(),
            NullLogger<WalletReception>.Instance,
            default);

        _ = fakeTgsRepository.GetWalletAsync(Arg.Is(sut.WalletId), Arg.Any<CancellationToken>())
            .Returns(new Wallet(
                sut.WalletId,
                Guid.NewGuid(),
                "test1",
                1000M,
                0M,
                Guid.NewGuid()));

        await sut.StartReceiveAsync();

        var command = new ReadCommand();

        await sut.EnqueueAsync(command);

        var actual = await command.GetBalanceResultAsync();

        Assert.AreEqual(1000M, actual.Balance);
        Assert.AreEqual(0M, actual.LockedBalance);
    }

    [Test]
    public async Task WalletReception_初始化錢包時如果資料還沒寫到資料庫_會更新到未寫入的最新值()
    {
        var services = new ServiceCollection();

        var fakeTgsRepository = Substitute.For<IWalletRepository>();

        var sut = new WalletReception(
            Guid.NewGuid(),
            services
                .AddSingleton(fakeTgsRepository)
                .BuildServiceProvider(),
            NullLogger<WalletReception>.Instance,
            default);

        var dbWallet = new Wallet(
            sut.WalletId,
            Guid.NewGuid(),
            "test1",
            1000M,
            0M,
            Guid.NewGuid());
        _ = fakeTgsRepository.GetWalletAsync(Arg.Is(sut.WalletId), Arg.Any<CancellationToken>())
            .Returns(dbWallet);

        _ = fakeTgsRepository.GetNewestRecordAsync(
            Arg.Is(sut.WalletId),
            Arg.Is(dbWallet.SnapshotId),
            Arg.Any<CancellationToken>())
            .Returns(new TransactionRecord(
                Guid.NewGuid(),
                sut.WalletId,
                dbWallet.BrandId,
                dbWallet.UserId,
                0M,
                0M,
                1234M,
                0M,
                TransactionType.Snapshot,
                null,
                DateTime.UtcNow,
                null,
                null));

        await sut.StartReceiveAsync();

        var command = new ReadCommand();

        await sut.EnqueueAsync(command);

        var actual = await command.GetBalanceResultAsync();

        Assert.AreEqual(1234M, actual.Balance);
        Assert.AreEqual(0M, actual.LockedBalance);
    }

    [Test]
    public async Task WalletReception_如果初始化的錢包是未升級到新機制的_會進行快照()
    {
        var services = new ServiceCollection();

        var fakeTgsRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var sut = new WalletReception(
            Guid.NewGuid(),
            services
                .AddSingleton(fakeTgsRepository)
                .AddSingleton(fakeSerialNumberGenerator)
                .BuildServiceProvider(),
            NullLogger<WalletReception>.Instance,
            default);

        _ = fakeTgsRepository.GetWalletAsync(Arg.Is(sut.WalletId), Arg.Any<CancellationToken>())
            .Returns(new Wallet(
                sut.WalletId,
                Guid.NewGuid(),
                "test1",
                1000M,
                0M,
                null));

        await sut.StartReceiveAsync();

        _ = fakeTgsRepository.Received(1)
            .WriteTransactionRecordAsync(
                Arg.Is<TransactionRecord>(record => record.Type == TransactionType.Snapshot
                    && record.Amount == 0M
                    && record.LockAmount == 0M
                    && record.Balance == 1000M
                    && record.LockedBalance == 0M),
                Arg.Any<CancellationToken>());
    }
}
