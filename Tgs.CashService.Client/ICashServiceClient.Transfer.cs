namespace Tgs.CashService.Client;

public partial interface ICashServiceClient
{
    ValueTask<TransactionResponse> LockTransferAsync(
        Guid walletId,
        decimal amount,
        decimal lockAmount,
        string? description,
        Guid externalId,
        Guid executor,
        CancellationToken cancellationToken = default);
}
