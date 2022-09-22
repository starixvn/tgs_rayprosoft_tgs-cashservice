using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransferRollbackCommandService : IGetService<TransferRollbackCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public TransferRollbackCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public ValueTask<ExchangeResult> GetAsync(TransferRollbackCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.TransferRollback.ToString(),
            Amount = command.Amount,
            LockAmount = -command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId.ToString(),
            CreatedUser = command.Executor
        };

        return _walletExchangeService.SendRequestAsync(request, cancellationToken);
    }
}
