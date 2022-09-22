using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class ReceiveCommitCommandService : IGetService<ReceiveCommitCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public ReceiveCommitCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public ValueTask<ExchangeResult> GetAsync(ReceiveCommitCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.ReceiveCommit.ToString(),
            Amount = command.Amount,
            LockAmount = -command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId.ToString(),
            CreatedUser = command.Executor
        };

        return _walletExchangeService.SendRequestAsync(request, cancellationToken);
    }
}
