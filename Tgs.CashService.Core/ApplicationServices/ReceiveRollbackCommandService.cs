using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class ReceiveRollbackCommandService : IGetService<ReceiveRollbackCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public ReceiveRollbackCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public ValueTask<ExchangeResult> GetAsync(ReceiveRollbackCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.ReceiveRollback.ToString(),
            Amount = 0M,
            LockAmount = -command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId.ToString(),
            CreatedUser = command.Executor
        };

        return _walletExchangeService.SendRequestAsync(request, cancellationToken);
    }
}
