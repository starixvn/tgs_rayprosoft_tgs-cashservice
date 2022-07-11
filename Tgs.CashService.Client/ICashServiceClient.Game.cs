namespace Tgs.CashService.Client;

public partial interface ICashServiceClient
{
    ValueTask<TransactionResponse> GameBetAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameSettleWinAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameSettleLoseAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameSettleFreeAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameSettleTieAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameCancelAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameLockAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameUnlockAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameWinJackpotAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameCreditAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameDebitAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameRefundAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameRevisionAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);
}
