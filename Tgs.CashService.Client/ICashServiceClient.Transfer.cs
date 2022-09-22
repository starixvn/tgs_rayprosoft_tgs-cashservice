namespace Tgs.CashService.Client;

public partial interface ICashServiceClient
{
    ValueTask<TransactionResponse> LockTransferAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid executor,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> CommitTransferAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid executor,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> RollbackTransferAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid executor,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> LockReceiveAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid executor,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> CommitReceiveAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid executor,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> RollbackReceiveAsync(
        Guid walletId,
        decimal amount,
        string? description,
        Guid externalId,
        Guid executor,
        CancellationToken cancellationToken = default);
}
