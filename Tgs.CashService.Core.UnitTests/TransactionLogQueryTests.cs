using NSubstitute;
using Tgs.CashService.ApplicationServices;
using Tgs.Paging;

namespace Tgs.CashService.Core.UnitTests;

public class TransactionLogQueryTests
{
    [Test]
    public async Task 查詢玩家帳號紀錄()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new TransactionLogQueryService(fakeTgsRepository);

        var query = new TransactionLogQuery(
            Guid.NewGuid(),
            (DateTime.UtcNow.AddDays(-1D), DateTime.UtcNow.AddDays(1D)),
            Array.Empty<TransactionType>(),
            new Paging<TransactionLog>
            {
                Page = 1,
                RowCount = 10
            });
        var playerCode = "test1";

        _ = fakeTgsRepository.GetPlayerCodeByWalletIdAsync(Arg.Is(query.WalletId))
            .Returns(playerCode);

        _ = await sut.QueryAsync(query).ToArrayAsync();

        _ = fakeTgsRepository.Received(1)
            .QueryTransactionLogsAsync(
                Arg.Is(query.WalletId),
                Arg.Is(playerCode),
                Arg.Is(query.QueryRange),
                Arg.Is(query.QueryTypes),
                Arg.Is(query.Paging));
    }

    [Test]
    public async Task 查詢玩家紀錄回傳筆數()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new TransactionLogQueryService(fakeTgsRepository);

        var query = new TransactionLogQuery(
            Guid.NewGuid(),
            (DateTime.UtcNow.AddDays(-1D), DateTime.UtcNow.AddDays(1D)),
            Array.Empty<TransactionType>(),
            new Paging<TransactionLog>
            {
                Page = 1,
                RowCount = 10
            });
        var playerCode = "test1";

        _ = fakeTgsRepository.GetPlayerCodeByWalletIdAsync(Arg.Is(query.WalletId))
            .Returns(playerCode);

        _ = fakeTgsRepository.CountTransactionLogsQueryAsync(
            Arg.Is(query.WalletId),
            Arg.Is(playerCode),
            Arg.Is(query.QueryRange),
            Arg.Is(query.QueryTypes))
            .Returns(100);

        var actual = await sut.GetAsync(query);

        Assert.AreEqual(100, actual);
    }

    [Test]
    public async Task 查詢玩家紀錄總金額()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new TransactionTotalAmountQueryService(fakeTgsRepository);

        var query = new TransactionTotalAmountQuery(
            Guid.NewGuid(),
            (DateTime.UtcNow.AddDays(-1D), DateTime.UtcNow.AddDays(1D)),
            Array.Empty<TransactionType>());
        var playerCode = "test1";

        _ = fakeTgsRepository.GetPlayerCodeByWalletIdAsync(Arg.Is(query.WalletId))
            .Returns(playerCode);

        _ = fakeTgsRepository.QueryTransactionTotalAmountAsync(
            Arg.Is(query.WalletId),
            Arg.Is(playerCode),
            Arg.Is(query.QueryRange),
            Arg.Is(query.QueryTypes))
            .Returns(100M);

        var actual = await sut.GetAsync(query);

        Assert.AreEqual(100M, actual);
    }
}
