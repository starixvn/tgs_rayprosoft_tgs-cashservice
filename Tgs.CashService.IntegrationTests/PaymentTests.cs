using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.CashService.Core;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class PaymentTests
{
    [TestMethod]
    public async Task Deposit_Amount_To_Wallet()
    {
        await using var baseServer = new WebApplicationFactory<Startup>();

        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = baseServer.WithWebHostBuilder(
            builder => builder.ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.Deposit),
                        Arg.Any<decimal>(),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns((transactionId, postBalance));

                services.AddSingleton(fakeWalletRepository);
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.DepositAsync(
            walletId,
            1000M,
            null,
            null);

        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Withdraw_Amount_From_Wallet()
    {
        await using var baseServer = new WebApplicationFactory<Startup>();

        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = baseServer.WithWebHostBuilder(
            builder => builder.ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.Withdrawal),
                        Arg.Any<decimal>(),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns((transactionId, postBalance));

                services.AddSingleton(fakeWalletRepository);
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.WithdrawalAsync(
            walletId,
            1000M,
            null,
            null);

        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }

    [TestMethod]
    public async Task Cancel_Withdraw_From_Wallet()
    {
        await using var baseServer = new WebApplicationFactory<Startup>();

        var walletId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var postBalance = 12345M;

        await using var web = baseServer.WithWebHostBuilder(
            builder => builder.ConfigureServices(services =>
            {
                var fakeWalletRepository = Substitute.For<IWalletRepository>();

                fakeWalletRepository.GetWalletAsync(Arg.Any<Guid>())
                    .Returns(new Wallet
                    {
                        Id = walletId,
                        BrandId = Guid.NewGuid(),
                        PlayerCode = "Player1"
                    });

                fakeWalletRepository
                    .ExchangeAsync(
                        Arg.Any<Guid>(),
                        Arg.Is(walletId),
                        Arg.Any<string>(),
                        Arg.Is(TransactionType.CancelWithdrawal),
                        Arg.Any<decimal>(),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<Guid?>())
                    .Returns((transactionId, postBalance));

                services.AddSingleton(fakeWalletRepository);
            }));

        var httpClient = web.CreateClient();

        var sut = new CashServiceClient(httpClient);

        var actual = await sut.CancelWithdrawalAsync(
            walletId,
            1000M,
            null,
            null,
            Guid.NewGuid());

        Assert.AreEqual(postBalance, actual.Balance);
        Assert.AreEqual(transactionId, actual.TransactionId);
    }
}
