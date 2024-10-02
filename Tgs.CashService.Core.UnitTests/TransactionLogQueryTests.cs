using NUnit.Framework.Legacy;
using Tgs.CashService.ApplicationServices;

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
            [],
            1,
            10);

        _ = await sut.GetAsync(query);

        _ = fakeTgsRepository.Received(1)
            .QueryTransactionLogsAsync(
                Arg.Is(query.WalletId),
                Arg.Is(query.QueryRange),
                Arg.Is(query.QueryTypes),
                Arg.Is(query.PageIndex),
                Arg.Is(query.PageSize));
    }

    [Test]
    public async Task 查詢玩家紀錄總金額()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();

        var sut = new TransactionTotalAmountQueryService(fakeTgsRepository);

        var query = new TransactionTotalAmountQuery(
            Guid.NewGuid(),
            (DateTime.UtcNow.AddDays(-1D), DateTime.UtcNow.AddDays(1D)),
            []);

        _ = fakeTgsRepository.QueryTransactionTotalAmountAsync(
            Arg.Is(query.WalletId),
            Arg.Is(query.QueryRange),
            Arg.Is(query.QueryTypes))
            .Returns(100M);

        var actual = await sut.GetAsync(query);

        ClassicAssert.AreEqual(100M, actual);
    }
}
