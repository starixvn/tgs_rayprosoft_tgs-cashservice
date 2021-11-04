using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Core.ApplicationServices;

namespace Tgs.CashService.Core.UnitTests
{
    [TestFixture]
    public class WithdrawalServiceUnitTest
    {
        [Test]
        public async Task Can_Create_Withdrawal_Exchange()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var faceLogger = NullLogger<WithdrawalService>.Instance;
            var sut = new WithdrawalService(faceLogger, fakeWalletRepository);

            var query = new WithdrawalQuery()
            {
                BrandId = Guid.NewGuid(),
                WalletId = Guid.NewGuid(),
                PlayerCode = "4ucmqplxb029",
                Amount = -100,
                ExternalId = Guid.NewGuid().ToString("N"),
                Description = "測試反水",
            };

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeWalletRepository
                .Received(1)
                .ExchangeAsync(
                    query.BrandId,
                    query.WalletId,
                    query.PlayerCode,
                    TransactionTypes.Withdrawal,
                    query.Amount,
                    query.Description,
                    query.ExternalId
                    );
        }
    }
}
