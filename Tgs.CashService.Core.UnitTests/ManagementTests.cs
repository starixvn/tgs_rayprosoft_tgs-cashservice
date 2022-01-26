using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests;

[TestFixture]
public class ManagementTests
{
    [Test]
    public async Task Can_Create_Adjust_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new AdjustCommandService(fakeWalletRepository, exchanger);

        var query = new AdjustCommand(
            Guid.NewGuid(),
            100M,
            Guid.NewGuid().ToString("N"),
            "測試調整",
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
                Arg.Is(TransactionType.Adjust),
                Arg.Is(query.Amount),
                Arg.Is(query.Description),
                Arg.Is(query.ExternalId),
                Arg.Is(query.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_Bonus_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new BonusCommandService(fakeWalletRepository, exchanger);

        var query = new BonusCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試紅利");

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
                Arg.Is(TransactionType.Bonus),
                Arg.Is(query.Amount),
                Arg.Is(query.Description),
                Arg.Is(query.ExternalId));
    }

    [Test]
    public async Task Can_Create_Rebate_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new RebateCommandService(fakeWalletRepository, exchanger);

        var query = new RebateCommand(
            Guid.NewGuid(),
            100M,
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

        _ = fakeWalletRepository.Received(1)
            .ExchangeAsync(
                Arg.Any<Guid>(),
                Arg.Is(query.WalletId),
                Arg.Any<string>(),
                Arg.Is(TransactionType.Rebate),
                Arg.Is(query.Amount),
                Arg.Is(query.Description),
                Arg.Is(query.ExternalId));
    }

    [Test]
    public async Task Can_Create_Other_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new OtherCommandService(fakeWalletRepository, exchanger);

        var query = new OtherCommand(
            Guid.NewGuid(),
            100,
            Guid.NewGuid().ToString("N"),
            "測試其他",
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
                Arg.Is(TransactionType.Other),
                Arg.Is(query.Amount),
                Arg.Is(query.Description),
                Arg.Is(query.ExternalId),
                Arg.Is(query.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_UnsettledBetManualCommand_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new UnsettledBetManualCommandService(fakeWalletRepository, exchanger);

        var command = new UnsettledBetManualCommand(
            Guid.NewGuid(),
            100,
            "測試Open Round Page",
            Guid.NewGuid().ToString("N"),
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
                Arg.Is(TransactionType.UnsettledBetManual),
                Arg.Is(command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId),
                Arg.Is(command.CreatedByUserId));
    }

    [Test]
    public async Task Can_Create_AdjustResettledBetManualCommand_Exchange()
    {
        var fakeWalletRepository = Substitute.For<IWalletRepository>();
        var fakeServiceProvider = Substitute.For<IServiceProvider>();
        var exchanger = new WalletExchanger(fakeServiceProvider);

        var sut = new AdjustResettledBetManualCommandService(fakeWalletRepository, exchanger);

        var command = new AdjustResettledBetManualCommand(
            Guid.NewGuid(),
            100,
            "測試Closed Round Page",
            Guid.NewGuid().ToString("N"),
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
                Arg.Is(TransactionType.AdjustResettledBetManual),
                Arg.Is(command.Amount),
                Arg.Is(command.Description),
                Arg.Is(command.ExternalId),
                Arg.Is(command.CreatedByUserId));
    }
}
