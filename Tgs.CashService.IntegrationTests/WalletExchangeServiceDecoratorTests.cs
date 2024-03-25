using System;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Routing.Handlers;
using Tgs.Cash.Protos;
using NATS.Client.Core;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class WalletExchangeServiceDecoratorTests
{
    [TestMethod]
    public async Task WalletExchangeServiceDecorator_執行交易要求()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();
        var sut = new WalletExchangeServiceDecorator(
            fakeWalletExchangeService,
            NullLogger<WalletExchangeServiceDecorator>.Instance);

        var request = new WalletExchangeRequest
        {
            WalletId = Guid.NewGuid(),
            Amount = -100M,
            LockAmount = 0M,
            TransactionType = "Test"
        };

        _ = await sut.SendRequestAsync(request);

        _ = fakeWalletExchangeService.Received(1)
            .SendRequestAsync(Arg.Is(request));
        _ = fakeWalletExchangeService.DidNotReceive()
            .DiscardBidAsync(Arg.Any<Guid>());
    }

    [TestMethod]
    public async Task WalletExchangeServiceDecorator_執行交易要求_如果發生NatsNoRespondersException會重新Bid重試一次()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();
        var sut = new WalletExchangeServiceDecorator(
            fakeWalletExchangeService,
            NullLogger<WalletExchangeServiceDecorator>.Instance);

        var walletId = Guid.NewGuid();
        var request = new WalletExchangeRequest
        {
            WalletId = walletId,
            Amount = -100M,
            LockAmount = 0M,
            TransactionType = "Test"
        };

        _ = fakeWalletExchangeService.SendRequestAsync(Arg.Is(request))
            .Returns(
                _ => throw new NatsNoRespondersException(),
                _ => new ExchangeResult(Guid.NewGuid(), 0M, 0M));

        _ = await sut.SendRequestAsync(request);

        _ = fakeWalletExchangeService.Received(2)
            .SendRequestAsync(Arg.Is(request));
        _ = fakeWalletExchangeService.Received(1)
            .DiscardBidAsync(Arg.Is(walletId));
    }

    [TestMethod]
    public async Task WalletExchangeServiceDecorator_執行交易要求_如果發生NatsNoReplyException會重新Bid重試一次()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();
        var sut = new WalletExchangeServiceDecorator(
            fakeWalletExchangeService,
            NullLogger<WalletExchangeServiceDecorator>.Instance);

        var walletId = Guid.NewGuid();
        var request = new WalletExchangeRequest
        {
            WalletId = walletId,
            Amount = -100M,
            LockAmount = 0M,
            TransactionType = "Test"
        };

        _ = fakeWalletExchangeService.SendRequestAsync(Arg.Is(request))
            .Returns(
                _ => throw new NatsNoReplyException(),
                _ => new ExchangeResult(Guid.NewGuid(), 0M, 0M));

        _ = await sut.SendRequestAsync(request);

        _ = fakeWalletExchangeService.Received(2)
            .SendRequestAsync(Arg.Is(request));
        _ = fakeWalletExchangeService.Received(1)
            .DiscardBidAsync(Arg.Is(walletId));
    }

    [TestMethod]
    public async Task WalletExchangeServiceDecorator_取得錢包餘額()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();
        var sut = new WalletExchangeServiceDecorator(
            fakeWalletExchangeService,
            NullLogger<WalletExchangeServiceDecorator>.Instance);

        var walletId = Guid.NewGuid();

        _ = await sut.GetBalanceAsync(walletId);

        _ = fakeWalletExchangeService.Received(1)
            .GetBalanceAsync(Arg.Is(walletId));
        _ = fakeWalletExchangeService.DidNotReceive()
            .DiscardBidAsync(Arg.Any<Guid>());
    }

    [TestMethod]
    public async Task WalletExchangeServiceDecorator_取得錢包餘額_如果發生NatsNoRespondersException會重新Bid重試一次()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();
        var sut = new WalletExchangeServiceDecorator(
            fakeWalletExchangeService,
            NullLogger<WalletExchangeServiceDecorator>.Instance);

        var walletId = Guid.NewGuid();

        _ = fakeWalletExchangeService.GetBalanceAsync(Arg.Is(walletId))
            .Returns(
                _ => throw new NatsNoRespondersException(),
                _ => 0M);

        _ = await sut.GetBalanceAsync(walletId);

        _ = fakeWalletExchangeService.Received(2)
            .GetBalanceAsync(Arg.Is(walletId));
        _ = fakeWalletExchangeService.Received(1)
            .DiscardBidAsync(Arg.Is(walletId));
    }

    [TestMethod]
    public async Task WalletExchangeServiceDecorator_取得錢包餘額_如果發生NatsNoReplyException會重新Bid重試一次()
    {
        var fakeWalletExchangeService = Substitute.For<IWalletExchangeService>();
        var sut = new WalletExchangeServiceDecorator(
            fakeWalletExchangeService,
            NullLogger<WalletExchangeServiceDecorator>.Instance);

        var walletId = Guid.NewGuid();

        _ = fakeWalletExchangeService.GetBalanceAsync(Arg.Is(walletId))
            .Returns(
                _ => throw new NatsNoReplyException(),
                _ => 0M);

        _ = await sut.GetBalanceAsync(walletId);

        _ = fakeWalletExchangeService.Received(2)
            .GetBalanceAsync(Arg.Is(walletId));
        _ = fakeWalletExchangeService.Received(1)
            .DiscardBidAsync(Arg.Is(walletId));
    }
}
