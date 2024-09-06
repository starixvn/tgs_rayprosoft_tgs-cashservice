using Microsoft.Extensions.Logging;
using Tgs.CashService.ApplicationServices;
using NUnit.Framework.Legacy;
using Tgs.CashService.Core.Entities;
using NSubstitute.ReturnsExtensions;
using Tgs.CashService.Models;
using NSubstitute.ExceptionExtensions;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderAdjustBalanceCommandServiceUnitTest
{
    [Test]
    public async Task GameProviderAdjustBalanceCommandService_WhenInvalidTransaction_ReturnErrorDuplicateTransaction()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderAdjustBalanceCommand>>();

        var sut = new GameProviderAdjustBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderAdjustBalanceCommand(
            "player_code",
            100M,
            0,
            null,
            "Description",
            "ExternalId");
        var transaction = new Transaction { Amount = 100M };
        fakeTgsRepository.GetTransactionLogByTypeAndExternalIdAsync(
            Arg.Is(TransactionType.GameProviderAdjustBalance),
            Arg.Is(command.ExternalId!))
            .Returns(transaction);

        var actual = await sut.GetAsync(command).ConfigureAwait(false);

        ClassicAssert.AreEqual(ErrorCode.DuplicateTransaction, actual.ErrorCode);
    }

    [Test]
    public async Task GameProviderAdjustBalanceCommandService_WhenInvalidPlayerCode_ReturnErrorNotFoundWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderAdjustBalanceCommand>>();

        var sut = new GameProviderAdjustBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderAdjustBalanceCommand(
            "player_code",
            100M,
            0,
            null,
            "Description",
            "ExternalId");
        fakeTgsRepository.GetTransactionLogByTypeAndExternalIdAsync(
            Arg.Is(TransactionType.GameProviderAdjustBalance),
            Arg.Is(command.ExternalId!))
            .ReturnsNull();
        fakeTgsRepository.ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == null
               && record.TransactionType == TransactionType.GameProviderAdjustBalance
               && record.Amount == command.AddedAmount
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
    public async Task GameProviderAdjustBalanceCommandService_WhenValidPlayerCodeAndAddedAmount_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderAdjustBalanceCommand>>();

        var sut = new GameProviderAdjustBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderAdjustBalanceCommand(
            "player_code",
            100M,
            0,
            null,
            "Description",
            "ExternalId");
        fakeTgsRepository.GetTransactionLogByTypeAndExternalIdAsync(
            Arg.Is(TransactionType.GameProviderAdjustBalance),
            Arg.Is(command.ExternalId!))
            .ReturnsNull();
        var exchangeResult = new ExchangeResult(Guid.Empty, 100,0);
        fakeTgsRepository.ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == null
               && record.TransactionType == TransactionType.GameProviderAdjustBalance
               && record.Amount == command.AddedAmount
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

    [Test]
    public async Task GameProviderAdjustBalanceCommandService_WhenValidPlayerCodeAndDeductedAmount_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderAdjustBalanceCommand>>();

        var sut = new GameProviderAdjustBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderAdjustBalanceCommand(
            "player_code",
            0,
            100M,
            null,
            "Description",
            "ExternalId");
        fakeTgsRepository.GetTransactionLogByTypeAndExternalIdAsync(
            Arg.Is(TransactionType.GameProviderAdjustBalance),
            Arg.Is(command.ExternalId!))
            .ReturnsNull();
        var exchangeResult = new ExchangeResult(Guid.Empty, 100, 0);
        fakeTgsRepository.ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == null
               && record.TransactionType == TransactionType.GameProviderAdjustBalance
               && record.Amount == -command.DeductedAmount
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
