using NSubstitute;
using NUnit.Framework;
using Tgs.CashServiceWorker;

namespace Tgs.CashService.Repositories.UnitTests;

[TestFixture]
public class WalletRepositoryUnitTest
{
    [Test]
    public async Task GetGameAsync()
    {
        var brandId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var playerCode = "4ucmqplxb029";
        var transactionType = TransactionType.Deposit;
        var amount = 100m;
        var description = "´ú¸Õ¦s´Ú";
        var externalId = Guid.NewGuid().ToString("N");
        var createdByUserId = Guid.NewGuid();

        var transactionId = Guid.NewGuid();
        var postBalance = 100m;
        var fakeRoundRepository = Substitute.For<IWalletRepository>();
        _ = fakeRoundRepository.ExchangeAsync(
            brandId,
            walletId,
            playerCode,
            transactionType,
            amount,
            0M,
            description,
            externalId,
            createdByUserId)
            .Returns(ValueTask.FromResult(new ExchangeResult(transactionId, postBalance, 0M)));
        (Guid TransactionId, decimal PostBalance, _) = await fakeRoundRepository.ExchangeAsync(
           brandId,
           walletId,
           playerCode,
           transactionType,
           amount,
           0M,
           description,
           externalId,
           createdByUserId).ConfigureAwait(false);
        Assert.AreEqual(transactionId, TransactionId);
        Assert.AreEqual(postBalance, PostBalance);
    }

    [Test]
    public async Task GetGameByExternalGameIdAsync()
    {
        var walletId = Guid.NewGuid();
        var balance = 0.0m;
        var fakeRoundRepository = Substitute.For<IWalletRepository>();
        _ = fakeRoundRepository.GetBalanceAsync(walletId).Returns(ValueTask.FromResult<decimal?>(balance));
        var result = await fakeRoundRepository.GetBalanceAsync(walletId).ConfigureAwait(false);
        Assert.AreEqual(balance, result);
    }
}
