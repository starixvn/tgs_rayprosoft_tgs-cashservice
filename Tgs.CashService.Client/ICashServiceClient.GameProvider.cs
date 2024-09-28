using Google.Protobuf.WellKnownTypes;

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

    ValueTask<BalanceResponse> GameProviderGetBalanceAsync(string playerCode, CancellationToken cancellationToken = default);

    ValueTask<BalanceResponse> GameProviderAddBalanceAsync(
        string playerCode,
        decimal amount,
        ProviderInfo providerInfo,
        string? description = "",
        CancellationToken cancellationToken = default);

    ValueTask<BalanceResponse> GameProviderDeductBalanceAsync(
        string playerCode,
        decimal amount,
        ProviderInfo providerInfo,
        string? description = "",
        CancellationToken cancellationToken = default);

     ValueTask<BalanceResponse> GameProviderAdjustBalanceAsync(
        string playerCode,
        decimal addedAmount,
        decimal deductedAmount,
        ProviderInfo providerInfo,
        string? description = "",
        CancellationToken cancellationToken = default);

    ValueTask<bool> IsExistGameProviderTransactionLogAsync(
        string playerCode,
        ProviderInfo providerInfo,
        CancellationToken cancellationToken = default);

    ValueTask<BalanceResponse> GameProviderAdjustBalanceByActionAsync(
       string playerCode,
       decimal amount,
       ProviderInfo providerInfo,
       bool isAllowNegative = false,
       string? description = "",
       CancellationToken cancellationToken = default);
}
