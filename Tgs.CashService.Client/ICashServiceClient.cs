namespace Tgs.CashService.Client;

public partial interface ICashServiceClient
{
    [Obsolete("舊版api")]
    ValueTask<TransactionResponse> AdjustCompanyAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> AdjustCompanyV2Async(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    [Obsolete("舊版api")]
    ValueTask<TransactionResponse> AdjustBonusAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> AdjustBonusV2Async(
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

    [Obsolete("舊版api")]
    ValueTask<TransactionResponse> AdjustAgentAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> AdjustAgentV2Async(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    [Obsolete("舊版api")]
    ValueTask<TransactionResponse> AdjustSystemAsync(
        Guid walletId,
        decimal amount,
        decimal? lockAmount,
        string? description,
        string? externalId,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> AdjustSystemV2Async(
        Guid walletId,
        decimal amount,
        decimal? lockAmount,
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

    ValueTask<PagingResponse<TransactionLog>> QueryTransactionLogsAsync(
        Guid walletId,
        DateTime from,
        DateTime to,
        IEnumerable<int> queryTransactionTypes,
        PagingConditions paging,
        CancellationToken cancellationToken = default);

    ValueTask<decimal?> QueryTransactionTotalAmountAsync(
        Guid walletId,
        DateTime from,
        DateTime to,
        IEnumerable<int> queryTransactionTypes,
        CancellationToken cancellationToken = default);

    ValueTask<Guid> CreateNewCashAsync(
        string userId,
        string currencyCode,
        CancellationToken cancellation = default);
}
