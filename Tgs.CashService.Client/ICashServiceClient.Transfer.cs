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
}
