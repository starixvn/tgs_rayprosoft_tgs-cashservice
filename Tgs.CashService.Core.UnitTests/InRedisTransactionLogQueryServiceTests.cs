using NUnit.Framework;
using NUnit.Framework.Legacy;
using Tgs.CashService.ApplicationServices;
using Tgs.Paging;

namespace Tgs.CashService.Core.UnitTests;

public class InRedisTransactionLogQueryServiceTests
{
    [Test]
    public async Task 查詢玩家還未寫入資料庫的帳變記錄()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new InRedisTransactionLogQueryService(fakeTgsRepository);

        var query = new InRedisTransactionLogQuery(
            Guid.NewGuid(),
            new Paging<TransactionLog>
            {
                Page = 1,
                RowCount = 10
            });

        _ = await sut.QueryAsync(query).ToArrayAsync();

        _ = fakeTgsRepository.Received(1)
            .QueryInRedisTransactionLogsAsync(Arg.Is(query.WalletId));
    }

    [Test]
    public async Task 查詢玩家還未寫入資料庫的帳變記錄_分頁查詢()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new InRedisTransactionLogQueryService(fakeTgsRepository);

        var query = new InRedisTransactionLogQuery(
            Guid.NewGuid(),
            new Paging<TransactionLog>
            {
                Page = 2,
                RowCount = 1
            });

        var data = new[]
        {
            new TransactionLog(
                Guid.NewGuid(),
                DateTime.UtcNow,
                TransactionType.Other,
                100M,
                200M,
                300M,
                400M,
                null,
                null),
            new TransactionLog(
                Guid.NewGuid(),
                DateTime.UtcNow,
                TransactionType.Other,
                100M,
                200M,
                300M,
                400M,
                null,
                null),
            new TransactionLog(
                Guid.NewGuid(),
                DateTime.UtcNow,
                TransactionType.Other,
                100M,
                200M,
                300M,
                400M,
                null,
                null)
        };

        _ = fakeTgsRepository.QueryInRedisTransactionLogsAsync(Arg.Is(query.WalletId))
            .Returns(data.ToAsyncEnumerable());

        var actual = await sut.QueryAsync(query).ToArrayAsync();

        ClassicAssert.AreEqual(1, actual.Length);
        ClassicAssert.AreEqual(data[1], actual.First());
    }

    [Test]
    public async Task 查詢玩家還未寫入資料庫的帳變記錄筆數()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new InRedisTransactionLogQueryService(fakeTgsRepository);

        var query = new InRedisTransactionLogQuery(
            Guid.NewGuid(),
            new Paging<TransactionLog>
            {
                Page = 1,
                RowCount = 10
            });

        _ = await sut.GetAsync(query);

        _ = fakeTgsRepository.Received(1)
            .CountInRedisTransactionLogsAsync(Arg.Is(query.WalletId));
    }
}
