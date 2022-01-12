using System;
using System.Threading.Tasks;

namespace Tgs.CashService.Client
{
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

        ValueTask<TransactionResponse> ResettledBetManualAsync(Guid walletId, decimal amount, string description,
            string externalId, Guid createdByUserId);

        ValueTask<TransactionResponse> AllAgentRebateAsync(Guid walletId, decimal amount, string? description,
            string? externalId);
    }
}