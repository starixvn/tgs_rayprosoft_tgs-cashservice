using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService;
using Tgs.CashServiceWorker.ApplicationServices;

namespace Tgs.CashServiceWorker.Core.UnitTests;

public class ExchangeWalletTests
{
    [Test]
    public async Task Exchange_Wallet()
    {
        var services = new ServiceCollection();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        services
            .AddSingleton<ILogger<WalletReception>>(NullLogger<WalletReception>.Instance)
            .AddSingleton(fakeWalletRepository);

        var exchanger = new WalletExchanger(services.BuildServiceProvider());
        var sut = new ExchangeWalletCommandService(exchanger);

        var command = new ExchangeWalletCommand(
            Guid.NewGuid(),
            TransactionType.AdjustCompany,
            100M,
            0M,
            null,
            null,
            null);

        fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        fakeWalletRepository.ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(command.WalletId),
                Arg.Any<string>(),
                Arg.Is(command.TransactionType),
                Arg.Is(command.Amount),
                Arg.Is(0M),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId),
                Arg.Is(command.CreatedByUserId))
            .Returns(new ExchangeResult(Guid.NewGuid(), 12345M, 0M));

        var actual = await sut.GetAsync(command);

        Assert.AreEqual(12345M, actual.Balance);
    }

    [Test]
    public async Task Get_Balance()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();

        var sut = new GetBalanceQueryService(fakeWalletRepository);

        var query = new GetBalanceQuery(Guid.NewGuid());

        fakeWalletRepository.GetBalanceAsync(Arg.Is(query.WalletId))
            .Returns(1000M);

        var actual = await sut.GetAsync(query);

        Assert.AreEqual(1000M, actual);
    }

    [Test]
    public void Create_Deposit_Exchange_With_Invalid_WalletId_Will_Throw_Exception()
    {
        var services = new ServiceCollection();

        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        services
            .AddSingleton<ILogger<WalletReception>>(NullLogger<WalletReception>.Instance)
            .AddSingleton(fakeWalletRepository);

        var exchanger = new WalletExchanger(services.BuildServiceProvider());
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

        _ = fakeWalletRepository.DidNotReceive()
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<TransactionType>(),
                Arg.Any<decimal>(),
                Arg.Any<decimal>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<Guid?>());
    }
}