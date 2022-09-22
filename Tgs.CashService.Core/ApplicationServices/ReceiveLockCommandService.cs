using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class ReceiveLockCommandService : IGetService<ReceiveLockCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public ReceiveLockCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public ValueTask<ExchangeResult> GetAsync(ReceiveLockCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.Receive.ToString(),
            Amount = 0M,
            LockAmount = command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId.ToString(),
            CreatedUser = command.Executor
        };

        return _walletExchangeService.SendRequestAsync(request, cancellationToken);
    }
}
