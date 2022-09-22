using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class TransferCommitCommandService : IGetService<TransferCommitCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public TransferCommitCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public ValueTask<ExchangeResult> GetAsync(TransferCommitCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.TransferCommit.ToString(),
            Amount = 0M,
            LockAmount = -command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId.ToString(),
            CreatedUser = command.Executor
        };

        return _walletExchangeService.SendRequestAsync(request, cancellationToken);
    }
}
