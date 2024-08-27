using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderGetBalanceCommandService(IWalletExchangeService walletExchangeService, ITgsRepository tgsRepository)
    : IGetService<GameProviderGetBalanceCommand, decimal>
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<decimal> GetAsync(GameProviderGetBalanceCommand command, CancellationToken cancellationToken = default)
    {
        Guid walletId = await tgsRepository.GetWalletIdAsync(command.PlayerCode, null, cancellationToken).ConfigureAwait(false);
        var balance = await _walletExchangeService
            .GetBalanceAsync(walletId, cancellationToken)
            .ConfigureAwait(false);

        return balance is null ? throw new InvalidOperationException("Can't found wallet.") : balance.Value;
    }
}
