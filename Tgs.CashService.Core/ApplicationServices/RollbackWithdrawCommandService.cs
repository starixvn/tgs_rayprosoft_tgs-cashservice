using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class RollbackWithdrawCommandService : IGetService<RollbackWithdrawCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public RollbackWithdrawCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public ValueTask<ExchangeResult> GetAsync(RollbackWithdrawCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.WithdrawRollback.ToString(),
            Amount = command.Amount,
            LockAmount = -command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        return _walletExchangeService
            .SendRequestAsync(request, cancellationToken);
    }
}
