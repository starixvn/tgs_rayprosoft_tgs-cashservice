using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class ManagementTests
    {
        [Test]
        public async Task Can_Create_Adjust_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();

            var sut = new AdjustService(fakeWalletRepository);

            var query = new AdjustCommand(
                Guid.NewGuid(),
                100M,
                Guid.NewGuid().ToString("N"),
                "´ú¸Õ½Õ¾ã",
                Guid.Parse("f76f8171-9d3b-ec11-bf5f-8c554ae8ab6b"));

            fakeWalletRepository.GetWalletAsync(Arg.Is(query.WalletId))
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
    }
}
