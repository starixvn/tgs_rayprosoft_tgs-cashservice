namespace Tgs.CashService.Client;

public interface ICashServiceClient
{
    ValueTask<TransactionResponse> AdjustAsync(
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

    ValueTask<TransactionResponse> AgentAdjustAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId);

    ValueTask<TransactionResponse> SystemAdjustAsync(
        Guid walletId,
        decimal amount,
        string? description,
        string? externalId,
        Guid createdByUserId);

    //ValueTask<TransactionResponse> PlaceBetAsync(
    //    Guid walletId,
    //    decimal amount,
    //    string? description,
    //    string? externalId);

    //ValueTask<TransactionResponse> WinBetAsync(
    //    Guid walletId,
    //    decimal amount,
    //    string? description,
    //    string? externalId);

    //ValueTask<TransactionResponse> LoseBetAsync(
    //    Guid walletId,
    //    decimal amount,
    //    string? description,
    //    string? externalId);

    //ValueTask<TransactionResponse> FreeBetAsync(
    //    Guid walletId,
    //    decimal amount,
    //    string? description,
    //    string? externalId);

    //ValueTask<TransactionResponse> TieBetAsync(
    //    Guid walletId,
    //    decimal amount,
    //    string? description,
    //    string? externalId);

    //ValueTask<TransactionResponse> CancelTransactionAsync(
    //    Guid walletId,
    //    decimal amount,
    //    string? description,
    //    string? externalId);

    //ValueTask<TransactionResponse> LockAmountAsync(
    //    Guid walletId,
    //    decimal amount,
    //    string? description,
    //    string? externalId);

    //ValueTask<TransactionResponse> UnlockAmountAsync(
    //    Guid walletId,
    //    decimal amount,
    //    string? description,
    //    string? externalId);
}