using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class GameProviderTests
    {
        [Test]
        public async Task Can_Create_GameProviderDeposit_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();

            var sut = new GameProviderDepositService(fakeWalletRepository);

            var query = new GameProviderDepositCommand(
                Guid.NewGuid(),
                100M,
                Guid.NewGuid().ToString("N"),
                "測試遊戲上分");

            fakeWalletRepository.GetWalletAsync(Arg.Is(query.WalletId))
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
                    Arg.Is(TransactionType.GameProviderDeposit),
                    Arg.Is(-query.Amount),
                    Arg.Is(query.Description),
                    Arg.Is(query.ExternalId));
        }

        [Test]
        public async Task Can_Create_GameProviderFundIn_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();

            var sut = new GameProviderFundInService(fakeWalletRepository);

            var query = new GameProviderFundInCommand(
                Guid.NewGuid(),
                100M,
                Guid.NewGuid().ToString("N"),
                "測試轉入主錢包");

            fakeWalletRepository.GetWalletAsync(Arg.Is(query.WalletId))
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

            var sut = new GameProviderFundOutService(fakeWalletRepository);

            var command = new GameProviderFundOutCommand(
                Guid.NewGuid(),
                100M,
                Guid.NewGuid().ToString("N"),
                "測試轉出主錢包");

            fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
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
            var faceLogger = NullLogger<GameProviderWithdrawalService>.Instance;
            var sut = new GameProviderWithdrawalService(faceLogger, fakeWalletRepository);

            var query = new GameProviderWithdrawalCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "4ucmqplxb029",
                100,
                Guid.NewGuid().ToString("N"),
                "測試遊戲下分");

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    query.BrandId,
                    query.WalletId,
                    query.PlayerCode,
                    TransactionType.GameProviderWithdrawal,
                    query.Amount,
                    query.Description,
                    query.ExternalId
                    );
        }
    }
}
