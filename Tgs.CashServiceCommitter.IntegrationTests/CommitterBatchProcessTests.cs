using Google.Protobuf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
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
        await sut.HandleAsync("test", request.ToByteArray(), null);
        await insertTcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await updateTcs.Task.WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        _ = fakeTgsRepository.Received(1)
            .InsertTransactionLogsAsync(Arg.Any<IEnumerable<TransactionRecord>>(), Arg.Any<CancellationToken>());
        _ = fakeTgsRepository.Received(1)
            .UpdateWalletAsync(Arg.Is(walletId), Arg.Any<CancellationToken>());
    }

    [Fact]
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

        var msg = Substitute.For<IAcknowledgeMessage<ReadOnlyMemory<byte>>>();
        _ = msg.Data.Returns(request.ToByteArray());

        // Act
        await sut.HandleAsync(msg);
        await executeTcs.Task.WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        _ = fakeCommandService.Received(1)
            .ExecuteAsync(
                Arg.Is(expectedCommand),
                Arg.Any<CancellationToken>());
    }
}
