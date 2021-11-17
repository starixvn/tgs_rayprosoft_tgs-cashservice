using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class PromotionTests
    {
        [Test]
        public async Task Can_Create_Promotion_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();

            var sut = new PromotionService(fakeWalletRepository);

            var command = new PromotionCommand(
                Guid.NewGuid(),
                100M,
                Guid.NewGuid().ToString("N"),
                "測試活動紅利");

            fakeWalletRepository.GetWalletAsync(Arg.Is(command.WalletId))
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
                    Arg.Is(TransactionType.Promotion),
                    Arg.Is(command.Amount),
                    Arg.Is(command.Description),
                    Arg.Is(command.ExternalId));
        }
    }
}
