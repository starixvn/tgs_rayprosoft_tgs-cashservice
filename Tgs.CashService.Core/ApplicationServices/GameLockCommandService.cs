using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameLockCommandService : IGetService<GameLockCommand, ExchangeResult>
{
    private readonly IWalletExchangeService _walletExchangeService;

    public GameLockCommandService(IWalletExchangeService walletExchangeService)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public async ValueTask<ExchangeResult> GetAsync(GameLockCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.GameLock.ToString(),
            Amount = -command.Amount,
            LockAmount = 0M,
            Description = string.Empty,
            ExternalId = command.ExternalId
        };

        return await _walletExchangeService
            .SendRequestAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
