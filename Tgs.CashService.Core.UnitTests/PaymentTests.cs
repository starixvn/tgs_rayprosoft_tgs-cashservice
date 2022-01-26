using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class PaymentTests
{
    [Test]
    public async Task Can_Create_Deposit_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new DepositCommandService(fakeWalletRepository, exchanger);

        var command = new DepositCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試存款",
            Guid.NewGuid());

        var fakeScope = Substitute.For<IServiceScope>();
        _ = fakeScope.ServiceProvider.Returns(fakeServiceProvider);
        var fakeScopeFactory = Substitute.For<IServiceScopeFactory>();
        _ = fakeScopeFactory.CreateScope()
            .Returns(fakeScope);

        _ = fakeServiceProvider.GetService(Arg.Any<Type>())
            .Returns(callInfo =>
            {
                var type = callInfo.Arg<Type>();

                return type switch
                {
                    Type logger when logger == typeof(ILogger<WalletReception>) => NullLogger<WalletReception>.Instance,
                    Type repos when repos == typeof(IWalletRepository) => fakeWalletRepository,
                    Type factory when factory == typeof(IServiceScopeFactory) => fakeScopeFactory,
                    _ => null
                };
            });

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                Id = command.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletRepository
            .Received(1)
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(command.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.Deposit),
                Arg.Is(command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId),
                Arg.Is(command.CreatedByUserId));
    }

    [Test]
    public void Create_Deposit_Exchange_With_Invalid_WalletId_Will_Throw_Exception()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new DepositCommandService(fakeWalletRepository, exchanger);

        var command = new DepositCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試存款",
            null);

        _ = Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.GetAsync(command));

        _ = fakeWalletRepository.DidNotReceive()
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(command.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.Deposit),
                Arg.Any<decimal>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<Guid?>());
    }

    [Test]
    public async Task Can_Create_Withdrawal_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new WithdrawalCommandService(fakeWalletRepository, exchanger);

        var query = new WithdrawalCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試反水");

        var fakeScope = Substitute.For<IServiceScope>();
        _ = fakeScope.ServiceProvider.Returns(fakeServiceProvider);
        var fakeScopeFactory = Substitute.For<IServiceScopeFactory>();
        _ = fakeScopeFactory.CreateScope()
            .Returns(fakeScope);

        _ = fakeServiceProvider.GetService(Arg.Any<Type>())
            .Returns(callInfo =>
            {
                var type = callInfo.Arg<Type>();

                return type switch
                {
                    Type logger when logger == typeof(ILogger<WalletReception>) => NullLogger<WalletReception>.Instance,
                    Type repos when repos == typeof(IWalletRepository) => fakeWalletRepository,
                    Type factory when factory == typeof(IServiceScopeFactory) => fakeScopeFactory,
                    _ => null
                };
            });

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(query.WalletId))
            .Returns(new Wallet
            {
                Id = query.WalletId,
                BrandId = Guid.NewGuid(),
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(query).ConfigureAwait(false);

        _ = fakeWalletRepository
            .Received(1)
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(query.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.Withdrawal),
                Arg.Is(-query.Amount),
                Arg.Is(query.Description),
                Arg.Is(query.ExternalId));
    }

    [Test]
    public async Task Can_Create_CancelWithdrawal_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new CancelWithdrawalCommandService(fakeWalletRepository, exchanger);

        var command = new CancelWithdrawalCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試取消提款單",
            Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

        var fakeScope = Substitute.For<IServiceScope>();
        _ = fakeScope.ServiceProvider.Returns(fakeServiceProvider);
        var fakeScopeFactory = Substitute.For<IServiceScopeFactory>();
        _ = fakeScopeFactory.CreateScope()
            .Returns(fakeScope);

        _ = fakeServiceProvider.GetService(Arg.Any<Type>())
            .Returns(callInfo =>
            {
                var type = callInfo.Arg<Type>();

                return type switch
                {
                    Type logger when logger == typeof(ILogger<WalletReception>) => NullLogger<WalletReception>.Instance,
                    Type repos when repos == typeof(IWalletRepository) => fakeWalletRepository,
                    Type factory when factory == typeof(IServiceScopeFactory) => fakeScopeFactory,
                    _ => null
                };
            });

        _ = fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
            .Returns(new Wallet
            {
                BrandId = Guid.NewGuid(),
                Id = command.WalletId,
                PlayerCode = "Player1"
            });

        _ = await sut.GetAsync(command).ConfigureAwait(false);

        _ = fakeWalletRepository
            .Received(1)
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(command.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.CancelWithdrawal),
                Arg.Is(command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId),
                Arg.Is(command.CreatedByUserId));
    }
}
