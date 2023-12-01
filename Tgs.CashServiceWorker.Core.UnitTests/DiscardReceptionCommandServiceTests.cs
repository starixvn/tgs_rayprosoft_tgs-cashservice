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

public class DiscardReceptionCommandServiceTests
{
    [Test]
    public async Task 取消錢包處理器()
    {
        using var cts = new CancellationTokenSource();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();

        var services = new ServiceCollection()
            .AddSingleton(Substitute.For<IMessageSender>())
            .AddSingleton((ILogger<WalletReception>)NullLogger<WalletReception>.Instance)
            .AddSingleton(fakeWalletRepository)
            .AddSingleton(Substitute.For<ISerialNumberGenerator>());

        var fakeHostApplicationLifetime = Substitute.For<IHostApplicationLifetime>();
        _ = fakeHostApplicationLifetime.ApplicationStopping
            .Returns(cts.Token);

        var exchanger = new WalletExchanger(
            services.BuildServiceProvider(),
            fakeHostApplicationLifetime);

        var sut = new DiscardReceptionCommandService(exchanger);

        var command = new DiscardReceptionCommand(Guid.NewGuid());

        _ = fakeWalletRepository.GetWalletAsync(
            Arg.Is(command.WalletId),
            Arg.Any<CancellationToken>())
            .Returns(new Wallet(
                command.WalletId,
                Guid.NewGuid(),
                "test1",
                1000M,
                500M,
                Guid.NewGuid()));

        _ = await exchanger.RequestAsync(new ExchangeRequest(
            command.WalletId,
            TransactionType.Deposit,
            500M,
            0M,
            null,
            null,
            null));

        await sut.ExecuteAsync(command);
        var actual = await exchanger.GetBalanceAsync(command.WalletId);

        ClassicAssert.AreEqual(1000M, actual.Balance);
    }
}
