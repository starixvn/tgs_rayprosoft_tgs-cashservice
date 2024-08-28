using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderGetBalanceCommandService(IWalletExchangeService walletExchangeService, ITgsRepository tgsRepository)
    : IGetService<GameProviderGetBalanceCommand, BalanceResult>
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<BalanceResult> GetAsync(GameProviderGetBalanceCommand command, CancellationToken cancellationToken = default)
    {
        Guid walletId = await _tgsRepository.GetWalletIdAsync(command.PlayerCode, null, cancellationToken).ConfigureAwait(false);
        var balance = await _walletExchangeService
            .GetBalanceAsync(walletId, cancellationToken)
            .ConfigureAwait(false);

        var isExistWallet = balance is not null;
        var errorCode = isExistWallet ? 0 : 1;
        var errorMessage = isExistWallet? string.Empty : "Can't found wallet.";
        return new BalanceResult(Guid.Empty, balance ?? 0, 0, errorCode, errorMessage);
    }
}
