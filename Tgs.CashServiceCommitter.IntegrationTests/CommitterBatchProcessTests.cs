using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NATS.Client.Core;
using Tgs.Cash.Protos;
using Tgs.CashService;
using Tgs.CashServiceCommitter.ApplicationServices;
using Tgs.CashServiceCommitter.MessageQueue;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;
using Xunit.Abstractions;

namespace Tgs.CashServiceCommitter.IntegrationTests;

public class CommitterBatchProcessTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task 處理交易記錄寫入要求()
    {
        // Arrange
        var fakeTransactionLogRepository = Substitute.For<ITransactionLogRepository>();
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeDistributeResourceManager = Substitute.For<IDistributedResourceManager>();

        await using var web = CashServiceCommitterTestHost.Host
            .WithWebHostBuilder(hostBuilder => hostBuilder
                .ConfigureLogging(logging => logging.AddXUnit(testOutputHelper))
                .ConfigureServices(services => services
                    .AddSingleton(fakeTransactionLogRepository)
                    .AddSingleton(fakeTgsRepository)
                    .AddSingleton(fakeDistributeResourceManager)));

        var walletId = Guid.NewGuid();
        var request = new CommitExchangeRequest
        {
            WalletId = walletId
        };

        var sut = ActivatorUtilities.CreateInstance<BookkeepHandler>(web.Services);

        var insertTcs = new TaskCompletionSource();

        fakeTgsRepository.When(fake => fake.InsertTransactionLogsAsync(
            Arg.Any<IEnumerable<TransactionRecord>>(),
            Arg.Any<CancellationToken>()))
        .Do(_ => insertTcs.SetResult());

        _ = fakeTransactionLogRepository.GetWaitingBookkeepLogsAsync(
            Arg.Is(walletId),
            Arg.Any<CancellationToken>())
            .Returns(
                new[] {
                    new TransactionRecord(
                        Guid.NewGuid(),
                        walletId,
                        Guid.NewGuid(),
                        "tester",
                        100M,
                        0M,
                        100M,
                        0M,
                        TransactionType.Deposit,
                        null,
                        DateTime.UtcNow,
                        null,
                        null)
                }.ToAsyncEnumerable(),
                AsyncEnumerable.Empty<TransactionRecord>());

        var updateTcs = new TaskCompletionSource();

        fakeTgsRepository.When(fake => fake.UpdateWalletAsync(
            Arg.Is(walletId),
            Arg.Any<CancellationToken>()))
            .Do(_ => updateTcs.SetResult());

        // Act
        await sut.HandleAsync("test", request, null);
        await insertTcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await updateTcs.Task.WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        _ = fakeTgsRepository.Received(1)
            .InsertTransactionLogsAsync(Arg.Any<IEnumerable<TransactionRecord>>(), Arg.Any<CancellationToken>());
        _ = fakeTgsRepository.Received(1)
            .UpdateWalletAsync(Arg.Is(walletId), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 2000)]
    public async Task 寫入要求處理競標機制()
    {
        // Arrange
        var fakeTransactionLogRepository = Substitute.For<ITransactionLogRepository>();
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeDistributeResourceManager = Substitute.For<IDistributedResourceManager>();
        var fakeCommandService = Substitute.For<ICommandService<BookkeepingWalletTransactionLogsCommand>>();

        await using var web = CashServiceCommitterTestHost.Host
            .WithWebHostBuilder(hostBuilder => hostBuilder
                .ConfigureLogging(logging => logging.AddXUnit(testOutputHelper))
                .ConfigureServices(services => services
                    .AddMessageQueue()
                    .AddDirectMessageQueue(config => config
                        .AddProcessor<BidProcessor>("tgs.cash.commit.bid")
                        .AddHandler<BookkeepHandler>($"tgs.cash.commit.batch.abc"))
                    .ClearAllExchanges()
                    .AddDirectGlobPatternExchange("*")
                    .Services
                    .AddSingleton(fakeTransactionLogRepository)
                    .AddSingleton(fakeTgsRepository)
                    .AddSingleton(fakeDistributeResourceManager)
                    .AddSingleton(fakeCommandService)));

        var walletId = Guid.NewGuid();
        var request = new CommitExchangeRequest
        {
            WalletId = walletId
        };

        var sut = ActivatorUtilities.CreateInstance<CommitExchangeDispatchHandler>(web.Services);

        _ = fakeDistributeResourceManager.BidAsync(
            Arg.Is(walletId),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns("abc");

        var executeTcs = new TaskCompletionSource();
        var expectedCommand = new BookkeepingWalletTransactionLogsCommand(walletId);
        fakeCommandService.When(fake => fake.ExecuteAsync(
            Arg.Is(expectedCommand),
            Arg.Any<CancellationToken>()))
            .Do(_ => executeTcs.SetResult());

        var msg = Substitute.For<IAcknowledgeMessage<CommitExchangeRequest>>();
        _ = msg.Data.Returns(request);

        // Act
        await sut.HandleAsync(msg);
        await executeTcs.Task;

        // Assert
        _ = fakeCommandService.Received(1)
            .ExecuteAsync(
                Arg.Is(expectedCommand),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CommitExchangeDispatchHandler_當發出要求出現NatsNoRespondersException_應該Discard後重新Bid()
    {
        // Arrange
        var fakeMessageSender = Substitute.For<IMessageSender>();
        var fakeDistributedResourceManager = Substitute.For<IDistributedResourceManager>();

        var sut = new CommitExchangeDispatchHandler(
            fakeMessageSender,
            fakeDistributedResourceManager,
            NullLogger<CommitExchangeDispatchHandler>.Instance);

        var walletId = Guid.NewGuid();
        var request = new CommitExchangeRequest
        {
            WalletId = walletId
        };

        var msg = Substitute.For<IAcknowledgeMessage<CommitExchangeRequest>>();
        _ = msg.Data.Returns(request);

        _ = fakeDistributedResourceManager.GetBidTokenAsync(Arg.Is(walletId))
            .Returns("test");

        _ = fakeMessageSender.SendAsync(Arg.Is("tgs.cash.commit.batch.test"), Arg.Is(request))
            .Returns(
                _ => throw new NatsNoRespondersException(),
                _ => ValueTask.CompletedTask);

        // Act
        await sut.HandleAsync(msg);

        // Assert
        _ = fakeDistributedResourceManager.Received(1)
            .DiscardBidAsync(Arg.Is(walletId));
        _ = msg.Received(1)
            .AckAsync();
    }
}
