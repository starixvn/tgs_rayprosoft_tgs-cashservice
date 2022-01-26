using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class GameProviderTests
{
    [Test]
    public async Task Can_Create_GameProviderDeposit_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new GameProviderDepositCommandService(fakeWalletRepository, exchanger);

        var command = new GameProviderDepositCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試遊戲上分");

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

        _ = fakeWalletRepository.Received(1)
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(command.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.GameProviderDeposit),
                Arg.Is(-command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderFundIn_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new GameProviderFundInCommandService(fakeWalletRepository, exchanger);

        var query = new GameProviderFundInCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試轉入主錢包");

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

        _ = fakeWalletRepository.Received(1)
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(query.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.GameProviderFundIn),
                Arg.Is(query.Amount),
                Arg.Is(query.Description),
                Arg.Is(query.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderFundOut_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new GameProviderFundOutCommandService(fakeWalletRepository, exchanger);

        var command = new GameProviderFundOutCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試轉出主錢包");

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
                Arg.Is(TransactionType.GameProviderFundOut),
                Arg.Is(-command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId));
    }

    [Test]
    public async Task Can_Create_GameProviderWithdrawal_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new GameProviderWithdrawalCommandService(fakeWalletRepository, exchanger);

        var command = new GameProviderWithdrawalCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試遊戲下分");

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
                Arg.Is(TransactionType.GameProviderWithdrawal),
                Arg.Is(command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId));
    }
}
