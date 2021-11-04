using System;
using System.Threading.Tasks;

namespace Tgs.CashService.Client
{
    public interface ICashServiceClinet
    {
        ValueTask<TransactionViewModel?> AdjustAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId, Guid CreatedByUserId);

        ValueTask<decimal> BalanceAsync(Guid walletId);

        ValueTask<TransactionViewModel?> BonusAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionViewModel?> CancelWithdrawalAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId, Guid CreatedByUserId);

        ValueTask<TransactionViewModel?> DepositAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionViewModel?> DividendAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionViewModel?> GameProviderDepositAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionViewModel?> GameProviderFundInAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionViewModel?> GameProviderFundOutAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionViewModel?> GameProviderWithdrawalAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionViewModel?> OtherAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId, Guid createdByUserId);

        ValueTask<TransactionViewModel?> PromotionAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionViewModel?> RebateAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId);

        ValueTask<TransactionViewModel?> WithdrawalAsync(Guid BrandId, Guid WalletId, string PlayerCode, decimal Amount, string? Description, string? ExternalId);
    }
}
