using System;
using System.Threading.Tasks;

namespace Tgs.CashService.Client
{
    public interface ICashServiceClient
    {
        ValueTask<TransactionResponse> AdjustAsync(
            Guid WalletId,
            decimal Amount,
            string? Description,
            string? ExternalId,
            Guid CreatedByUserId);

        ValueTask<decimal> GetBalanceAsync(Guid walletId);

        ValueTask<TransactionResponse> BonusAsync(Guid WalletId, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionResponse> CancelWithdrawalAsync(
            Guid WalletId,
            decimal Amount,
            string? Description,
            string? ExternalId,
            Guid? CreatedByUserId);

        ValueTask<TransactionResponse> DepositAsync(Guid WalletId, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionResponse> GameProviderDepositAsync(Guid WalletId, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionResponse> GameProviderFundInAsync(Guid WalletId, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionResponse> GameProviderFundOutAsync(Guid WalletId, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionResponse> GameProviderWithdrawalAsync(Guid WalletId, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionResponse> OtherAsync(Guid WalletId, decimal Amount, string Description, string ExternalId, Guid createdByUserId);

        ValueTask<TransactionResponse> PromotionAsync(
            Guid WalletId,
            decimal Amount,
            string? Description,
            string? ExternalId,
            Guid? createByUserId);

        ValueTask<TransactionResponse> RebateAsync(Guid WalletId, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionResponse> WithdrawalAsync(Guid WalletId, decimal Amount, string? Description, string? ExternalId);
    }
}
