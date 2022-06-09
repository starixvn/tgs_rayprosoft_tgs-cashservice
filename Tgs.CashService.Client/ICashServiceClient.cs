namespace Tgs.CashService.Client;

public partial interface ICashServiceClient
{
    ValueTask<TransactionResponse> AdjustCompanyAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> AdjustBonusAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<decimal> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> BonusAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

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

    ValueTask<TransactionResponse> GameProviderWithdrawalAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> OtherAsync(
        Guid walletId,
        decimal amount,
        string description,
        string externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> PromotionAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid? createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> RebateAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> UnsettledBetManualAsync(
        Guid walletId,
        decimal amount,
        string description,
        string externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> AdjustResettledBetManualAsync(
        Guid walletId,
        decimal amount,
        string description,
        string externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> DividendAllAgentRebateAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> PromotionVipLevelUpgradeAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> PromotionVipLevelSeasonAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> PromotionVipLevelBirthdayAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> AdjustAgentAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> AdjustSystemAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> AdjustTransferAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GamePlaceBetAsync(
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

    ValueTask<TransactionResponse> GameCancelTransactionAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameLockAmountAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameUnlockAmountAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> GameWinJackpotAsync(
        Guid walletId,
        decimal amount,
        string externalId,
        CancellationToken cancellationToken = default);
}
