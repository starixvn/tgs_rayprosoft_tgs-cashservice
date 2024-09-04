using Microsoft.Extensions.Logging;
using Tgs.CashService.ApplicationServices;
using NUnit.Framework.Legacy;
using Tgs.CashService.Core.Entities;
using NSubstitute.ReturnsExtensions;
using Tgs.CashService.Models;
using NSubstitute.ExceptionExtensions;

namespace Tgs.CashService.Core.UnitTests.GameProviders;

[TestFixture]
public class GameProviderDeductBalanceCommandServiceUnitTest
{
    [Test]
    public async Task GameProviderDeductBalanceCommandService_WhenAmountLessThanOrEqual0_ReturnErrorInvalidAmount()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderDeductBalanceCommand>>();

        var sut = new GameProviderDeductBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderDeductBalanceCommand(
            "player_code",
            -100M,
            null,
            "Description",
            string.Empty);

        var actual = await sut.GetAsync(command).ConfigureAwait(false);

        ClassicAssert.AreEqual(ErrorCodes.InvalidAmount, actual.ErrorCode);
    }

    [Test]
    public async Task GameProviderDeductBalanceCommandService_WhenInvalidTransaction_ReturnErrorDuplicateTransaction()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderDeductBalanceCommand>>();

        var sut = new GameProviderDeductBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderDeductBalanceCommand(
            "player_code",
            100M,
            null,
            "Description",
            "ExternalId");
        var transaction = new Transaction { Amount = 100M };
        fakeTgsRepository.GetTransactionLogByTypeAndExternalIdAsync(
            Arg.Is(TransactionType.GameProviderDeductBalance),
            Arg.Is(command.ExternalId!))
            .Returns(transaction);

        var actual = await sut.GetAsync(command).ConfigureAwait(false);

        ClassicAssert.AreEqual(ErrorCodes.DuplicateTransaction, actual.ErrorCode);
    }



    [Test]
    public async Task GameProviderDeductBalanceCommandService_WhenInvalidPlayerCode_ReturnErrorNotFoundWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderDeductBalanceCommand>>();

        var sut = new GameProviderDeductBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderDeductBalanceCommand(
            "player_code",
            100M,
            null,
            "Description",
            "ExternalId");
        fakeTgsRepository.GetTransactionLogByTypeAndExternalIdAsync(
            Arg.Is(TransactionType.GameProviderDeductBalance),
            Arg.Is(command.ExternalId!))
            .ReturnsNull();
        fakeTgsRepository.ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == null
               && record.TransactionType == TransactionType.GameProviderDeductBalance
               && record.Amount == -command.Amount
               && record.LockAmount == 0
               && record.Description == command.Description
               && record.ExternalId == command.ExternalId
               && record.UserId == null
               && record.PlayerCode == command.PlayerCode))
           .Throws(new NotFoundWalletException("testing"));

        var actual = await sut.GetAsync(command).ConfigureAwait(false);

        ClassicAssert.AreEqual(ErrorCodes.NotFoundWallet, actual.ErrorCode);
    }

    [Test]
    public async Task GameProviderDeductBalanceCommandService_WhenInsufficientFunds_ReturnErrorInsufficientFunds()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderDeductBalanceCommand>>();

        var sut = new GameProviderDeductBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderDeductBalanceCommand(
            "player_code",
            100M,
            null,
            "Description",
            "ExternalId");
        fakeTgsRepository.GetTransactionLogByTypeAndExternalIdAsync(
            Arg.Is(TransactionType.GameProviderDeductBalance),
            Arg.Is(command.ExternalId!))
            .ReturnsNull();
        fakeTgsRepository.ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == null
               && record.TransactionType == TransactionType.GameProviderDeductBalance
               && record.Amount == -command.Amount
               && record.LockAmount == 0
               && record.Description == command.Description
               && record.ExternalId == command.ExternalId
               && record.UserId == null
               && record.PlayerCode == command.PlayerCode))
           .Throws(new InsufficientFundException("testing"));

        var actual = await sut.GetAsync(command).ConfigureAwait(false);

        ClassicAssert.AreEqual(ErrorCodes.InsufficientFunds, actual.ErrorCode);
    }

    [Test]
    public async Task GameProviderDeductBalanceCommandService_WhenValidPlayerCode_ExchangeWallet()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeLogger = Substitute.For<ILogger<GameProviderDeductBalanceCommand>>();

        var sut = new GameProviderDeductBalanceCommandService(fakeTgsRepository, fakeLogger);

        var command = new GameProviderDeductBalanceCommand(
            "player_code",
            100M,
            null,
            "Description",
            "ExternalId");
        fakeTgsRepository.GetTransactionLogByTypeAndExternalIdAsync(
            Arg.Is(TransactionType.GameProviderDeductBalance),
            Arg.Is(command.ExternalId!))
            .ReturnsNull();
        var exchangeResult = new ExchangeResult(Guid.Empty, 100, 0);
        fakeTgsRepository.ExchangeWalletAsync(Arg.Is<ExchangeWalletRecord>(
               record => record.WalletId == null
               && record.TransactionType == TransactionType.GameProviderDeductBalance
               && record.Amount == -command.Amount
               && record.LockAmount == 0
               && record.Description == command.Description
               && record.ExternalId == command.ExternalId
               && record.UserId == null
               && record.PlayerCode == command.PlayerCode))
           .Returns(exchangeResult);

        var actual = await sut.GetAsync(command).ConfigureAwait(false);

        ClassicAssert.AreEqual(ErrorCodes.Success, actual.ErrorCode);
        ClassicAssert.AreEqual(100, actual.Balance);
        ClassicAssert.AreEqual(0, actual.LockedBalance);
    }
}
