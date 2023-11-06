using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

[Obsolete("已淘汰")]
internal class WithdrawalCommandService : IGetService<WithdrawalCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public WithdrawalCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public async ValueTask<ExchangeResult> GetAsync(WithdrawalCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.Withdrawal.ToString(),
            Amount = -command.Amount,
            LockAmount = 0M,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        return await _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
