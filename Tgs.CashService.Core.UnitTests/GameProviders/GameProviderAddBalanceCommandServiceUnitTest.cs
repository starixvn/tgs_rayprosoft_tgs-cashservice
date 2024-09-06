using Microsoft.Extensions.Logging;
using Tgs.CashService.ApplicationServices;
using NUnit.Framework.Legacy;
using Tgs.CashService.Core.Entities;
using NSubstitute.ReturnsExtensions;
using Tgs.CashService.Models;
using NSubstitute.ExceptionExtensions;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderAddBalanceCommandServiceUnitTest
{
    [Test]
    public async Task GameProviderAddBalanceCommandService_WhenAmountLessThanOrEqual0_ReturnErrorInvalidAmount()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderAddBalanceCommand>>();

        var sut = new GameProviderAddBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderAddBalanceCommand(
            "player_code",
            -100M,
            null,
            "Description",
            string.Empty);

        var actual = await sut.GetAsync(command).ConfigureAwait(false);

        ClassicAssert.AreEqual(ErrorCode.InvalidAmount, actual.ErrorCode);
    }

    [Test]
    public async Task GameProviderAddBalanceCommandService_WhenInvalidTransaction_ReturnErrorDuplicateTransaction()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderAddBalanceCommand>>();

        var sut = new GameProviderAddBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderAddBalanceCommand(
            "player_code",
            100M,
            null,
            "Description",
            "ExternalId");
        var transaction = new Transaction { Amount = 100M };
        fakeTgsRepository.GetTransactionLogByTypeAndExternalIdAsync(
            Arg.Is(TransactionType.GameProviderAddBalance),
            Arg.Is(command.ExternalId!))
            .Returns(transaction);

        var actual = await sut.GetAsync(command).ConfigureAwait(false);

        ClassicAssert.AreEqual(ErrorCode.DuplicateTransaction, actual.ErrorCode);
    }



    [Test]
    public async Task GameProviderAddBalanceCommandService_WhenInvalidPlayerCode_ReturnErrorNotFoundWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderAddBalanceCommand>>();

        var sut = new GameProviderAddBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderAddBalanceCommand(
            "player_code",
            100M,
            null,
            "Description",
            "ExternalId");
        fakeTgsRepository.GetTransactionLogByTypeAndExternalIdAsync(
            Arg.Is(TransactionType.GameProviderAddBalance),
            Arg.Is(command.ExternalId!))
            .ReturnsNull();
        fakeTgsRepository.ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == null
               && record.TransactionType == TransactionType.GameProviderAddBalance
               && record.Amount == command.Amount
               && record.LockAmount == 0
               && record.Description == command.Description
               && record.ExternalId == command.ExternalId
               && record.UserId == null
               && record.PlayerCode == command.PlayerCode))
           .Throws(new Exception("testing"));

        var actual = await sut.GetAsync(command).ConfigureAwait(false);

        ClassicAssert.AreEqual(ErrorCode.NotFoundWallet, actual.ErrorCode);
    }

    [Test]
    public async Task GameProviderAddBalanceCommandService_WhenValidPlayerCode_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderAddBalanceCommand>>();

        var sut = new GameProviderAddBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderAddBalanceCommand(
            "player_code",
            100M,
            null,
            "Description",
            "ExternalId");
        fakeTgsRepository.GetTransactionLogByTypeAndExternalIdAsync(
            Arg.Is(TransactionType.GameProviderAddBalance),
            Arg.Is(command.ExternalId!))
            .ReturnsNull();
        var exchangeResult = new ExchangeResult(Guid.Empty, 100,0);
        fakeTgsRepository.ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == null
               && record.TransactionType == TransactionType.GameProviderAddBalance
               && record.Amount == command.Amount
               && record.LockAmount == 0
               && record.Description == command.Description
               && record.ExternalId == command.ExternalId
               && record.UserId == null
               && record.PlayerCode == command.PlayerCode))
           .Returns(exchangeResult);

        var actual = await sut.GetAsync(command).ConfigureAwait(false);

        ClassicAssert.AreEqual(ErrorCode.Success, actual.ErrorCode);
        ClassicAssert.AreEqual(100, actual.Balance);
        ClassicAssert.AreEqual(0, actual.LockedBalance);
    }
}
