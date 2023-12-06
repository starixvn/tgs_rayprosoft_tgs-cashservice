using System.Diagnostics.Metrics;
using ExpectedObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework.Legacy;
using Tgs.CashService;
using Tgs.CashServiceWorker.ApplicationServices;
using Tgs.SerialNumberGenerator;
using Valhalla.MessageQueue;

namespace Tgs.CashServiceWorker.Core.UnitTests;

public class ExchangeWalletTests
{
    [Test]
    public async Task Exchange_Wallet()
    {
        var services = new ServiceCollection();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        var fakeMessageSender = Substitute.For<IMessageSender>();

        _ = services
            .AddSingleton<ILogger<WalletReception>>(NullLogger<WalletReception>.Instance)
            .AddSingleton(fakeWalletRepository)
            .AddSingleton(fakeSerialNumberGenerator)
            .AddSingleton(fakeMessageSender);

        var fakeHostApplicationLifetime = Substitute.For<IHostApplicationLifetime>();

        var fakeMeterFactory = new StubMeterFactory();

        var exchanger = new WalletExchanger(
            services.BuildServiceProvider(),
            fakeHostApplicationLifetime,
            fakeMeterFactory);
        var sut = new ExchangeWalletCommandService(exchanger);

        var command = new ExchangeWalletCommand(
            Guid.NewGuid(),
            TransactionType.AdjustCompany,
            100M,
            0M,
            null,
            null,
            null);

        var brandId = Guid.NewGuid();
        var userId = "Player1";
        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId), Arg.Any<CancellationToken>())
            .Returns(new Wallet(
                command.WalletId,
                brandId,
                userId,
                0M,
                0M,
                Guid.NewGuid()));

        var actual = await sut.GetAsync(command);

        var expectedRecord = new TransactionRecord(
            actual.TransactionId,
            command.WalletId,
            brandId,
            userId,
            command.Amount,
            command.LockAmount,
            100M,
            0M,
            command.TransactionType,
            command.Description,
            DateTime.UtcNow,
            command.ExternalId,
            command.CreatedByUserId).ToExpectedObject(ctx => ctx
                .Ignore(o => o.RecordedOn)
                .IgnoreEqualsOverride());

        ClassicAssert.AreEqual(100M, actual.Balance);
        ClassicAssert.AreEqual(0M, actual.LockedBalance);

        _ = fakeWalletRepository.Received(1)
            .WriteTransactionRecordAsync(
                Arg.Is<TransactionRecord>(record => expectedRecord.Matches(record)),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Get_Balance()
    {
        var services = new ServiceCollection();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        var fakeMessageSender = Substitute.For<IMessageSender>();

        _ = services
            .AddSingleton<ILogger<WalletReception>>(NullLogger<WalletReception>.Instance)
            .AddSingleton(fakeWalletRepository)
            .AddSingleton(fakeSerialNumberGenerator)
            .AddSingleton(fakeMessageSender);

        var fakeHostApplicationLifetime = Substitute.For<IHostApplicationLifetime>();

        var fakeMeterFactory = new StubMeterFactory();

        var exchanger = new WalletExchanger(
            services.BuildServiceProvider(),
            fakeHostApplicationLifetime,
            fakeMeterFactory);
        var sut = new GetBalanceQueryService(exchanger);

        var query = new GetBalanceQuery(Guid.NewGuid());

        _ = fakeWalletRepository.GetWalletAsync(
            Arg.Is(query.WalletId),
            Arg.Any<CancellationToken>())
            .Returns(new Wallet(
                query.WalletId,
                Guid.NewGuid(),
                "test1",
                1000M,
                0M,
                Guid.NewGuid()));

        var actual = await sut.GetAsync(query);

        ClassicAssert.AreEqual(1000M, actual);
    }

    [Test]
    public void Create_Deposit_Exchange_With_Invalid_WalletId_Will_Throw_Exception()
    {
        var services = new ServiceCollection();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        _ = services
            .AddSingleton<ILogger<WalletReception>>(NullLogger<WalletReception>.Instance)
            .AddSingleton(fakeWalletRepository);
        var fakeHostApplicationLifetime = Substitute.For<IHostApplicationLifetime>();

        var fakeMeterFactory = new StubMeterFactory();

        var exchanger = new WalletExchanger(
            services.BuildServiceProvider(),
            fakeHostApplicationLifetime,
            fakeMeterFactory);
        var sut = new ExchangeWalletCommandService(exchanger);

        var command = new ExchangeWalletCommand(
            Guid.NewGuid(),
            TransactionType.AdjustCompany,
            100M,
            0M,
            null,
            null,
            null);

        _ = Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.GetAsync(command));
    }
}
