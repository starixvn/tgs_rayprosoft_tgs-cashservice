namespace Tgs.CashService.Client;

public partial interface ICashServiceClient
{
    ValueTask<TransactionResponse> GameProviderDepositAsync(
       Guid walletId,
       decimal amount,
       string? description,
       string? externalId,
       CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameProviderFundInAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameProviderFundOutAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameProviderFundRefundAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameProviderWithdrawalAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameProviderAddBalanceAsync(
        string playerCode,
        decimal amount,
        string? description = "",
        string? externalId = null,
        Guid? walletId = null,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameProviderDeductBalanceAsync(
        string playerCode,
        decimal amount,
        string? description = "",
        string? externalId = null,
        Guid? walletId = null,
        CancellationToken cancellationToken = default);
}
