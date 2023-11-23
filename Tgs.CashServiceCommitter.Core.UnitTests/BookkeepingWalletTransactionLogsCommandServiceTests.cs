using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.CashService;
using Tgs.CashServiceCommitter.ApplicationServices;
using Valhalla.Messages;

namespace Tgs.CashServiceCommitter.Core.UnitTests;
public class BookkeepingWalletTransactionLogsCommandServiceTests
{
    [Fact]
    public async Task 寫入交易記錄到資料庫()
    {
        // Arrange
        var fakeTranscationLogRepository = Substitute.For<ITransactionLogRepository>();
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeEventBus = Substitute.For<IEventBus>();

        var sut = new BookkeepingWalletTransactionLogsCommandService(
            fakeTranscationLogRepository,
            fakeTgsRepository,
            fakeEventBus,
            NullLogger<BookkeepingWalletTransactionLogsCommandService>.Instance);

        var command = new BookkeepingWalletTransactionLogsCommand(Guid.NewGuid());

        _ = fakeTranscationLogRepository.GetWaitingBookkeepLogsAsync(Arg.Is(command.WalletId))
            .Returns(
                new[] {
                    new TransactionRecord(
                        Guid.NewGuid(),
                        command.WalletId,
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

        var fakeWalletUpdateEventSource = Substitute.For<IEventSource<WalletUpdateEvent>>();

        _ = fakeEventBus.GetEventSource<WalletUpdateEvent>(Arg.Is(CommitterConsts.WalletUpdateEventName))
            .Returns(fakeWalletUpdateEventSource);

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        _ = fakeTgsRepository.Received(1)
            .InsertTransactionLogsAsync(
                Arg.Is<IEnumerable<TransactionRecord>>(records => records.Count() == 1));
        _ = fakeTranscationLogRepository.Received(1)
            .RemoveBookkeepLogsAsync(Arg.Is(command.WalletId), Arg.Is(1));
        _ = fakeWalletUpdateEventSource.Received(1)
            .PublishAsync(Arg.Is(new WalletUpdateEvent(command.WalletId)));
    }

    [Fact]
    public async Task 寫入交易記錄到資料庫時_如果批次發生錯誤_會用Migrate方式再試()
    {
        // Arrange
        var fakeTranscationLogRepository = Substitute.For<ITransactionLogRepository>();
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeEventBus = Substitute.For<IEventBus>();

        var sut = new BookkeepingWalletTransactionLogsCommandService(
            fakeTranscationLogRepository,
            fakeTgsRepository,
            fakeEventBus,
            NullLogger<BookkeepingWalletTransactionLogsCommandService>.Instance);

        var command = new BookkeepingWalletTransactionLogsCommand(Guid.NewGuid());

        _ = fakeTranscationLogRepository.GetWaitingBookkeepLogsAsync(Arg.Is(command.WalletId))
            .Returns(
                new[] {
                    new TransactionRecord(
                        Guid.NewGuid(),
                        command.WalletId,
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

        _ = fakeTgsRepository.InsertTransactionLogsAsync(Arg.Any<IEnumerable<TransactionRecord>>())
            .Throws<Exception>();

        var fakeWalletUpdateEventSource = Substitute.For<IEventSource<WalletUpdateEvent>>();

        _ = fakeEventBus.GetEventSource<WalletUpdateEvent>(Arg.Is(CommitterConsts.WalletUpdateEventName))
            .Returns(fakeWalletUpdateEventSource);

        // Act
        await sut.ExecuteAsync(command);

        // Assert
        _ = fakeTgsRepository.Received(1)
            .MigrateTransactionLogsAsync(Arg.Any<IEnumerable<TransactionRecord>>());
        _ = fakeTranscationLogRepository.Received(1)
            .RemoveBookkeepLogsAsync(Arg.Is(command.WalletId), Arg.Is(1));
        _ = fakeWalletUpdateEventSource.Received(1)
            .PublishAsync(Arg.Is(new WalletUpdateEvent(command.WalletId)));
    }
}
