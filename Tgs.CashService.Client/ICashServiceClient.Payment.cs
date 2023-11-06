namespace Tgs.CashService.Client;

public partial interface ICashServiceClient
{
    [Obsolete("已淘汰")]
    ValueTask<TransactionResponse> CancelWithdrawalAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createdByUserId,
        CancellationToken cancellationToken = default);

    [Obsolete("已淘汰")]
    ValueTask<TransactionResponse> DepositAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> DepositedAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createdByUserId,
        CancellationToken cancellationToken = default);

    [Obsolete("已淘汰")]
    ValueTask<TransactionResponse> WithdrawalAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> LockWithdrawAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> CommitWithdrawAsync(
        Guid walletId,
        decimal amount,
        decimal actualAmount,
        string? description,
        string? externalId,
        Guid? createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> RollbackWithdrawAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createdByUserId,
        CancellationToken cancellationToken = default);
}
