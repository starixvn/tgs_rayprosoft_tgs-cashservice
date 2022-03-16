namespace Tgs.CashService.Client;

public interface ICashServiceClient
{
    ValueTask<TransactionResponse> AdjustCompanyAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId);

    ValueTask<decimal> GetBalanceAsync(Guid walletId);

    ValueTask<TransactionResponse> BonusAsync(Guid walletId, decimal amount, string? description,
        string? externalId);

    ValueTask<TransactionResponse> CancelWithdrawalAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createdByUserId);

    ValueTask<TransactionResponse> DepositAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createdByUserId);

    ValueTask<TransactionResponse> GameProviderDepositAsync(Guid walletId, decimal amount, string? description,
        string? externalId);

    ValueTask<TransactionResponse> GameProviderFundInAsync(Guid walletId, decimal amount, string? description,
        string? externalId);

    ValueTask<TransactionResponse> GameProviderFundOutAsync(Guid walletId, decimal amount, string? description,
        string? externalId);

    ValueTask<TransactionResponse> GameProviderWithdrawalAsync(Guid walletId, decimal amount, string? description,
        string? externalId);

    ValueTask<TransactionResponse> OtherAsync(Guid walletId, decimal amount, string description, string externalId,
        Guid createdByUserId);

    ValueTask<TransactionResponse> PromotionAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createdByUserId);

    ValueTask<TransactionResponse> RebateAsync(Guid walletId, decimal amount, string? description,
        string? externalId);

    ValueTask<TransactionResponse> WithdrawalAsync(Guid walletId, decimal amount, string? description,
        string? externalId);

    ValueTask<TransactionResponse> UnsettledBetManualAsync(Guid walletId, decimal amount, string description,
        string externalId, Guid createdByUserId);

    ValueTask<TransactionResponse> AdjustResettledBetManualAsync(Guid walletId, decimal amount, string description,
        string externalId, Guid createdByUserId);

    ValueTask<TransactionResponse> DividendAllAgentRebateAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId);

    ValueTask<TransactionResponse> PromotionVipLevelUpgradeAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId);

    ValueTask<TransactionResponse> PromotionVipLevelSeasonAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId);

    ValueTask<TransactionResponse> AdjustAgentAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId);

    ValueTask<TransactionResponse> AdjustSystemAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId);

    ValueTask<TransactionResponse> AdjustTransferAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId);

    ValueTask<TransactionResponse> GameProviderPlaceBetAsync(
        Guid walletId,
        decimal amount,
        string externalId);

    ValueTask<TransactionResponse> GameProviderSettleWinAsync(
        Guid walletId,
        decimal amount,
        string externalId);

    ValueTask<TransactionResponse> GameProviderSettleLoseAsync(
        Guid walletId,
        decimal amount,
        string externalId);

    ValueTask<TransactionResponse> GameProviderSettleFreeAsync(
        Guid walletId,
        decimal amount,
        string externalId);

    ValueTask<TransactionResponse> GameProviderSettleTieAsync(
        Guid walletId,
        decimal amount,
        string externalId);

    ValueTask<TransactionResponse> GameProviderCancelPlaceBetAsync(
        Guid walletId,
        decimal amount,
        string externalId);

    ValueTask<TransactionResponse> GameProviderCancelSettlementAsync(
        Guid walletId,
        decimal amount,
        string externalId);

    ValueTask<TransactionResponse> GameProviderLockAmountAsync(
        Guid walletId,
        decimal amount,
        string externalId);

    ValueTask<TransactionResponse> GameProviderUnlockAmountAsync(
        Guid walletId,
        decimal amount,
        string externalId);
}
