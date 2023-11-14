using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.Cash.Protos;
using Tgs.CashService.Client;
using Tgs.CashServiceWorker;

namespace Tgs.CashService.IntegrationTests.GameProviders;

[TestClass]
public class GameProviderFundRefundTest
{
    [TestMethod]
    public async Task GameProvider_FundRefund_To_Wallet()
    {
        var walletId = Guid.NewGuid();
        var amount = 1000M;
        var description = "轉帳錢包退款";
        var externalId = Guid.NewGuid().ToString();
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = new TestCashApplication(builder => builder
            .ConfigureServices(services =>
            {
                _ = services.AddSingleton<WalletExchanger>();

                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                _ = fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                _ = fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.GameProviderFundRefund),
                        Arg.Is(amount),
                        Arg.Is(0M),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>(),
                        Arg.Any<CancellationToken>())
                    .Returns(new ExchangeResult(transactionId, postBalance, 0M));

                _ = services.AddSingleton(fakeWalletRepository);

                var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();

                _ = services.AddSingleton(sp =>
                {
                    var exchanger = sp.GetRequiredService<WalletExchanger>();

                    _ = fakeWalletExchangeService.SendRequestAsync(Arg.Any<WalletExchangeRequest>(), Arg.Any<CancellationToken>())
                        .Returns(callInfo =>
                        {
                            var req = callInfo.Arg<WalletExchangeRequest>();

                            return exchanger.RequestAsync(new ExchangeRequest(
                                req.WalletId,
                                Enum.Parse<TransactionType>(req.TransactionType),
                                req.Amount,
                                req.LockAmount,
                                req.Description,
                                req.ExternalId,
                                req.CreatedUser)).Result;
                        });

                    return fakeWalletExchangeService;
                });

                _ = services.AddFakeNatsMessageQueue();
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.GameProviderFundRefundAsync(
            walletId,
            amount,
            description: description,
            externalId: externalId);

        Assert.IsNotNull(actual);
        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }
}
